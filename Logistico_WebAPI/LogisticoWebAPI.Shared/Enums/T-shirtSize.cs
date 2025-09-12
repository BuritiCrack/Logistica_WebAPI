using System.ComponentModel;

namespace LogisticoWebAPI.Shared.Enums
{
    public enum TShirtSize
    {
        [Description("S - Pequeña")]
        S,

        [Description("M - Mediana")]
        M,

        [Description("L - Grande")]
        L,

        [Description("XL - Extra Grande")]
        XL,

        [Description("XXL - Doble Extra Grande")]
        XXL
    }
}