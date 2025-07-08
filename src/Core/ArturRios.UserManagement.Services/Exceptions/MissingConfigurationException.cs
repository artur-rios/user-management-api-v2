using ArturRios.Common.Output;

namespace ArturRios.UserManagement.Services.Exceptions;

public class MissingConfigurationException(string[] messages, string message = "Internal error") : CustomException(messages, message);
