using System;

namespace AdminCli.Configuration;

public record TokenInfo(string AccessToken, DateTime ValidTo);
