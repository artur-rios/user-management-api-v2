using TechCraftsmen.Core.Output;

namespace TechCraftsmen.Management.User.Services.Exceptions;

public class MissingConfigurationException(string[] messages, string message = "Internal error") : CustomException(messages, message);
