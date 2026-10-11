package adns

import (
	"fmt"
	"strings"
	"sync"
	"time"

	"github.com/miekg/dns"
)

const (
	maxCacheTTL     = 60 * time.Second
	maxCacheEntries = 4096
)

type cacheEntry struct {
	msg *dns.Msg
	exp time.Time
}

var (
	cacheMu sync.Mutex
	cache   = map[string]cacheEntry{}
)

// exchange is Client.Exchange with a response cache: each upstream query opens a new TCP/TLS
// connection, so repeats are worth answering locally. Only NOERROR answers are cached, for
// min(record TTLs, 60 s). ponytail: TTLs in cached replies are not decremented; full map flush at
// maxCacheEntries; add per-entry eviction or TTL aging if it matters.
func exchange(c *dns.Client, m *dns.Msg, addr string) (*dns.Msg, error) {
	if len(m.Question) != 1 {
		r, _, err := c.Exchange(m, addr)
		return r, err
	}

	q := m.Question[0]
	do := false
	if o := m.IsEdns0(); o != nil {
		do = o.Do()
	}
	key := fmt.Sprintf("%s|%s|%d|%d|%t", addr, strings.ToLower(q.Name), q.Qtype, q.Qclass, do)

	cacheMu.Lock()
	e, ok := cache[key]
	cacheMu.Unlock()
	if ok && time.Now().Before(e.exp) {
		r := e.msg.Copy()
		r.Id = m.Id
		return r, nil
	}

	r, _, err := c.Exchange(m, addr)
	if err != nil || r.Rcode != dns.RcodeSuccess || len(r.Answer) == 0 || r.Truncated {
		return r, err
	}

	ttl := maxCacheTTL
	for _, rr := range r.Answer {
		if t := time.Duration(rr.Header().Ttl) * time.Second; t < ttl {
			ttl = t
		}
	}
	if ttl > 0 {
		cacheMu.Lock()
		if len(cache) >= maxCacheEntries {
			cache = map[string]cacheEntry{}
		}
		cache[key] = cacheEntry{r.Copy(), time.Now().Add(ttl)}
		cacheMu.Unlock()
	}
	return r, nil
}
