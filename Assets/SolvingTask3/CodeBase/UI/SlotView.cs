using System;
using System.Linq;
using AxGrid.Base;
using UnityEngine;
using UnityEngine.UI;

namespace SolvingTask3.CodeBase.UI
{
    [Serializable]
    public class ItemRaritySprite
    {
        [field:SerializeField] public ItemRarity Rarity { get; private set; }
        [field:SerializeField] public Sprite Sprite { get; private set; }
    }
    public class SlotView : MonoBehaviourExt
    {
        [SerializeField] private ItemRaritySprite[] _raritySprites;
        [SerializeField] private Image _image;

        public ItemRarity CurrentRarity { get; private set; }
        
        public void SetItemRarity(ItemRarity rarityType)
        {
            CurrentRarity = rarityType;
            _image.sprite = _raritySprites.First(s => s.Rarity == rarityType).Sprite;
        }
    }
}