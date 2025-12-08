namespace ShapezShifter.Hijack
{
    public interface ISaveDataRewirer : IRewirer
    {
        void OnSave(ISavegameBlobWriter writer);
        void OnLoad(SavegameBlobReader reader);
    }
}
