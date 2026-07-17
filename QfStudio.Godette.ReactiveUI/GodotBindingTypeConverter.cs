using ReactiveUI;

namespace QfStudio.Godette.ReactiveUI;

public class FloatToDoubleConverter : BindingTypeConverter<float, double>
{
    public override int GetAffinityForObjects() => 5;

    public override bool TryConvert(float from, object? conversionHint, out double result)
    {
        result = from;
        return true;
    }
}

public class DoubleToFloatConverter : BindingTypeConverter<double, float>
{
    public override int GetAffinityForObjects() => 5;

    public override bool TryConvert(double from, object? conversionHint, out float result)
    {
        result = (float)from;
        return true;
    }
}

// TODO add tests
public class EnumToStringConverter<TEnum> : BindingTypeConverter<TEnum, string>
    where TEnum : struct, Enum
{
    public override int GetAffinityForObjects() => 5;

    public override bool TryConvert(TEnum from, object? conversionHint, out string result)
    {
        result = from.ToString();
        return true;
    }
}

// TODO add tests
public class StringToEnumConverter<TEnum> : BindingTypeConverter<string, TEnum>
    where TEnum : struct, Enum
{
    public override int GetAffinityForObjects() => 5;

    public override bool TryConvert(string? from, object? conversionHint, out TEnum result)
    {
        if (from is not null && Enum.TryParse<TEnum>(from, ignoreCase: true, out result))
            return true;

        result = default;
        return false;
    }
}

public class VariantToIntConverter : BindingTypeConverter<Godot.Variant, int>
{
    public override int GetAffinityForObjects() => 5;

    public override bool TryConvert(Godot.Variant from, object? conversionHint, out int result)
    {
        result = from.AsInt32();
        return true;
    }
}

public class VariantToFloatConverter : BindingTypeConverter<Godot.Variant, float>
{
    public override int GetAffinityForObjects() => 5;

    public override bool TryConvert(Godot.Variant from, object? conversionHint, out float result)
    {
        result = from.AsSingle();
        return true;
    }
}

public class VariantToDoubleConverter : BindingTypeConverter<Godot.Variant, double>
{
    public override int GetAffinityForObjects() => 5;

    public override bool TryConvert(Godot.Variant from, object? conversionHint, out double result)
    {
        result = from.AsDouble();
        return true;
    }
}

public class VariantToStringConverter : BindingTypeConverter<Godot.Variant, string>
{
    public override int GetAffinityForObjects() => 5;

    public override bool TryConvert(Godot.Variant from, object? conversionHint, out string result)
    {
        result = from.AsString();
        return true;
    }
}

public class VariantToBoolConverter : BindingTypeConverter<Godot.Variant, bool>
{
    public override int GetAffinityForObjects() => 5;

    public override bool TryConvert(Godot.Variant from, object? conversionHint, out bool result)
    {
        result = from.AsBool();
        return true;
    }
}

public class IntToVariantConverter : BindingTypeConverter<int, Godot.Variant>
{
    public override int GetAffinityForObjects() => 5;

    public override bool TryConvert(int from, object? conversionHint, out Godot.Variant result)
    {
        result = from;
        return true;
    }
}

public class FloatToVariantConverter : BindingTypeConverter<float, Godot.Variant>
{
    public override int GetAffinityForObjects() => 5;

    public override bool TryConvert(float from, object? conversionHint, out Godot.Variant result)
    {
        result = from;
        return true;
    }
}

public class DoubleToVariantConverter : BindingTypeConverter<double, Godot.Variant>
{
    public override int GetAffinityForObjects() => 5;

    public override bool TryConvert(double from, object? conversionHint, out Godot.Variant result)
    {
        result = from;
        return true;
    }
}

public class StringToVariantConverter : BindingTypeConverter<string, Godot.Variant>
{
    public override int GetAffinityForObjects() => 5;

    public override bool TryConvert(string? from, object? conversionHint, out Godot.Variant result)
    {
        result = from is null ? default : (Godot.Variant)from;
        return true;
    }
}

public class BoolToVariantConverter : BindingTypeConverter<bool, Godot.Variant>
{
    public override int GetAffinityForObjects() => 5;

    public override bool TryConvert(bool from, object? conversionHint, out Godot.Variant result)
    {
        result = from;
        return true;
    }
}
