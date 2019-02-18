using System.Collections;
using System.Collections.Generic;

public enum ItemType
{
    Normal,
    Indicator,
    Acid,
    Base,
    Salt
}

public enum ItemList
{
    pH_Paper,
    Turmeric,
    Vermilion,
    Bromythol_Blue,
    HCl,
    NaOh,
    NaCl,
    Hybiscus
}

public enum IndicatorsList
{
    pH_Paper = ItemList.pH_Paper,
    Turmeric = ItemList.Turmeric,
    Vermilion = ItemList.Vermilion,
    Bromythol_Blue = ItemList.Bromythol_Blue
}

public enum AcidsList
{
    HCl = ItemList.HCl,
    H2SO4,
    C6H8O7
}
public enum BasesList
{
    NaOH = ItemList.NaOh
}
public enum SaltsList
{
    Null,
    NaCl = ItemList.NaCl
}
public enum NormalItemList
{
    Hybiscus = ItemList.Hybiscus,
    Lemon
}


