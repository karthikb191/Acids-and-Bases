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
    HCl = ItemList.HCl
}
public enum BasesList
{
    NaOh = ItemList.NaOh
}
public enum SaltsList
{
    NaCl = ItemList.NaCl
}
public enum NormalItemList
{
    Hybiscus = ItemList.Hybiscus
}


