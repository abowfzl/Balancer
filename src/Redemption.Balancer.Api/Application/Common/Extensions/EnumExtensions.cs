using Newtonsoft.Json;
using NpgsqlTypes;
using Redemption.Balancer.Api.Domain.Enums;

namespace Redemption.Balancer.Api.Application.Common.Extensions;

public static class EnumExtensions
{
    public static string? GetEnumPgName<T>(this T enumValue)
        where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
            return null;

        var pgName = enumValue.ToString();
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString() ?? string.Empty);

        if (fieldInfo != null)
        {
            var attrs = fieldInfo.GetCustomAttributes(typeof(PgNameAttribute), true);
            if (attrs.Length > 0)
            {
                pgName = ((PgNameAttribute)attrs[0]).PgName;
            }
        }

        return pgName;
    }

    public static Role ConvertToRoleEnum(this string roleValue)
    {
        return roleValue?.ToLower() switch
        {
            "client" => Role.Client,
            "admin" => Role.Admin,
            "semitrusted_client" => Role.SemitrustedClient,
            "distrusted_client" => Role.DistrustedClient,
            "trusted_client" => Role.TrustedClient,
            _ => throw new JsonSerializationException($"Invalid role value: {roleValue}")
        };
    }
}