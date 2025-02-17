[System.Serializable]
public struct CharacterAttribute 
{
    // mac m1 ??????? windows §Õ????
    // ????????????????,????¦Ã?,?????????? mac ??????

    public CharacterPartAnimator characterPart;
    public PartVariantColor partVariantColour;
    public PartVarianType partVarianType;

    public CharacterAttribute (CharacterPartAnimator characterPart , PartVariantColor partVariantColour , PartVarianType partVarianType)
    {
        this.characterPart = characterPart;
        this.partVariantColour = partVariantColour;
        this.partVarianType = partVarianType;
    }
}
