package main

import (
	"testing"
	"time"

	"github.com/miekg/dns"
)

type fakeWriter struct {
	dns.ResponseWriter
	written *dns.Msg
}

func (f *fakeWriter) WriteMsg(m *dns.Msg) error {
	// the real writer packs the message and panics on nil
	if _, err := m.Pack(); err != nil {
		return err
	}
	f.written = m
	return nil
}

func TestUpstreamFailureAnswersServFail(t *testing.T) {
	query := new(dns.Msg)
	query.SetQuestion("example.com.", dns.TypeA)

	for name, handle := range map[string]func(dns.ResponseWriter, *dns.Msg){
		"other": func(w dns.ResponseWriter, m *dns.Msg) {
			ODNS = dns.Client{Net: "tcp", Timeout: time.Second}
			OtherDNS = "127.0.0.1:1" // nothing listens here
			handleOtherDNS(w, m)
		},
		"china": func(w dns.ResponseWriter, m *dns.Msg) {
			CDNS = dns.Client{Net: "tcp", Timeout: time.Second}
			ChinaDNS = "127.0.0.1:1"
			handleChinaDNS(w, m)
		},
	} {
		w := &fakeWriter{}
		handle(w, query)

		if w.written == nil || w.written.Rcode != dns.RcodeServerFailure || w.written.Id != query.Id {
			t.Fatalf("%s: got %+v, want a SERVFAIL reply to the query", name, w.written)
		}
	}
}
