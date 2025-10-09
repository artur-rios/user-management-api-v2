using ArturRios.Common.Output;

namespace ArturRios.UserManagement.Services.Exceptions;

public class MissingConfigurationException(string[] messages) : CustomException(messages);
