namespace EnhancedDictionaryEditor.Model
{
    public class SaveData
    {
        public ItemInfos[] Added { get; set; }
        public ItemInfos[] Updated { get; set; }
        public ItemInfos[] Deleted { get; set; }
    }
}
