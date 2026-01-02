using System.Reflection;

namespace Magidesk.Migrations.Seeding;

internal static class ReflectionUtil
{
    public static void SetProperty(object target, string propertyName, object? value)
    {
        var prop = target.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (prop == null)
        {
            throw new InvalidOperationException($"Property '{propertyName}' not found on {target.GetType().FullName}.");
        }

        // Handle non-public setters (common with DDD entities)
        var setMethod = prop.GetSetMethod(nonPublic: true);
        if (setMethod != null)
        {
            setMethod.Invoke(target, new[] { value });
            return;
        }

        // Fallback: try compiler-generated backing field for auto-properties (walk type hierarchy)
        for (var t = target.GetType(); t != null; t = t.BaseType)
        {
            var backingField = t.GetField(
                $"<{propertyName}>k__BackingField",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (backingField != null)
            {
                backingField.SetValue(target, value);
                return;
            }
        }

        throw new ArgumentException($"Property set method not found for '{propertyName}' on {target.GetType().FullName}.");
    }

    public static TField GetField<TField>(object target, string fieldName)
    {
        var field = target.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        if (field == null)
        {
            throw new InvalidOperationException($"Field '{fieldName}' not found on {target.GetType().FullName}.");
        }

        return (TField)field.GetValue(target)!;
    }
}


