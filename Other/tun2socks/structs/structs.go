package structs

// ServerInfo describes one outbound server; tun_dial fills it field by field.
type ServerInfo struct {
	Type      string
	Hostname  string
	Username  string
	Password  string
	Method    string
	Prot      string
	ProtParam string
	OBFS      string
	OBFSParam string
}
