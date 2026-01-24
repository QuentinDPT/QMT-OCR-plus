using System.Text.Json.Serialization;
using System.Text.Json;

namespace QMTGroup.Urn;

public class UrnConverter : JsonConverter<Urn>
{
    // Sérialiser Urn en tant que chaîne (utilisée comme clé dans un dictionnaire)
    public override void Write(Utf8JsonWriter writer, Urn value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString()); // Sérialiser l'Urn comme une chaîne
    }

    // Désérialiser Urn à partir de chaîne
    public override Urn Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var urnValue = reader.GetString();
        return new Urn(urnValue ?? "urn:NA");
    }

    // Ce qui suit permet de gérer la sérialisation des clés de dictionnaire
    public override void WriteAsPropertyName(Utf8JsonWriter writer, Urn value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(value.ToString()); // Utiliser la valeur de l'URN comme nom de propriété dans un dictionnaire
    }

    public override Urn ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var urnValue = reader.GetString();
        return new Urn(urnValue ?? "urn:NA"); // Désérialiser la clé de dictionnaire en tant qu'URN
    }
}
