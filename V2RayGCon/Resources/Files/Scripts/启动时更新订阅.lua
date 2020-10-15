local proxyPort = Web:GetProxyPort()
Web:UpdateSubscriptions(proxyPort)
Server:UpdateAllSummary()