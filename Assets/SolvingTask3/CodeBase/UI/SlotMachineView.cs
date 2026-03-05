using System;
using System.Collections.Generic;
using System.Linq;
using AxGrid;
using AxGrid.Base;
using AxGrid.Model;
using AxGrid.Path;
using SolvingTask3.CodeBase.Constants;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SolvingTask3.CodeBase.UI
{
    public class SlotMachineView : MonoBehaviourExtBind
    {
        [Header("References")]
        [SerializeField] private string _speedFieldName = SlotModelKeys.CurrentSpeed;
        [SerializeField] private Transform _slotsParent;
        [SerializeField] private Transform _stopTarget;
        [SerializeField] private GameObject _starBurstParticle;
        [SerializeField] private GameObject _slotPrefab;
        
        [Header("Settings")]
        [SerializeField] private float _scaleEffect = 1.5f;
        [SerializeField] private float _scaleEffectRange = 500;
        [SerializeField] private float _slotHeight = 200f;
        [SerializeField] private float _offsetBetweenSlots = 50f;
        [SerializeField] private Vector2 _referenceResolution = new(1920, 1080);

        private List<SlotView> _slots;
        private float _totalHeight;
        private float _screenScaleFactor;

        [OnAwake]
        private void Init()
        {
            CalculateSizesForCurrentScreen();
            
            _slots = CreateSlots();
            
            _totalHeight = _slots.Count * _slotHeight + _offsetBetweenSlots * (_slots.Count - 1);
        }
        
        [OnUpdate]
        private void UpdatePosition()
        {
            float speed = Model.GetFloat(_speedFieldName);
            
            if(speed >= 0.01f)
                MoveSlots(speed * _screenScaleFactor * Time.deltaTime);
        }

        [Bind(EventNames.StartSpinning)]
        private void OnStartSpinning()
        {
            _starBurstParticle.SetActive(false);
        }
        
        [Bind(EventNames.FinalAlignmentStarted)]
        private void OnFinalAlignmentStarted()
        {
            SlotView targetSlot = _slots
                .OrderBy(s => Mathf.Abs(s.transform.position.y - _stopTarget.position.y))
                .First();
    
            float neededDelta = targetSlot.transform.position.y - _stopTarget.position.y;
            float duration = 0.3f;
            
            float startValue = neededDelta / (duration / Time.deltaTime);
            
            CreateNewPath()
                .EasingLinear(duration, startValue, 0f, MoveSlots)
                .Action(() =>
                {
                    _starBurstParticle.SetActive(true);
                    Settings.Invoke(EventNames.SlotMachineStopped, targetSlot.CurrentRarity);
                });
        }

        private void MoveSlots(float delta)
        {
            foreach (SlotView slot in _slots)
            {
                Vector2 position = slot.transform.position;
                position.y -= delta;

                if (position.y <= _slotsParent.position.y - (_offsetBetweenSlots * 2 + _slotHeight * 2))
                {
                    position.y += _totalHeight + _offsetBetweenSlots;
                    SetRandomItemRarity(slot);
                }
                
                slot.transform.position = position;
                ApplyScaleEffect(slot);
            }
        }

        private List<SlotView> CreateSlots()
        {
            List<SlotView> result = new List<SlotView>();

            float posY = _slotsParent.transform.position.y + (_offsetBetweenSlots * 2 + _slotHeight * 2);
            
            for (int i = 0; i < 5; i++)
            {
                Vector3 pos = new Vector3(_slotsParent.position.x, posY, _slotsParent.position.z);
                
                SlotView slot = Instantiate(_slotPrefab, pos, Quaternion.identity, _slotsParent)
                    .GetComponent<SlotView>();
                
                SetRandomItemRarity(slot);
                ApplyScaleEffect(slot);

                result.Add(slot);
                
                posY -= _offsetBetweenSlots + _slotHeight;
            }
            
            return result;
        }

        private static void SetRandomItemRarity(SlotView slot)
        {
            ItemRarity[] rarityTypes = Enum.GetValues(typeof(ItemRarity)).Cast<ItemRarity>().ToArray();
            slot.SetItemRarity(rarityTypes[Random.Range(0, rarityTypes.Length)]);
        }

        private void CalculateSizesForCurrentScreen()
        {
            float currentScreenHeight = Screen.height;
            float referenceScreenHeight = _referenceResolution.y;
            
            _screenScaleFactor = currentScreenHeight / referenceScreenHeight;
            
            _scaleEffectRange *= _screenScaleFactor;
            _slotHeight *= _screenScaleFactor;
            _offsetBetweenSlots *= _screenScaleFactor;
        }
        
        private void ApplyScaleEffect(SlotView slot)
        {
            float distanceToTarget = Mathf.Abs(slot.transform.position.y - _stopTarget.position.y);
            
            slot.transform.localScale =  Vector3.one * CalculateScaleEffect(distanceToTarget, 0.01f, 
                _scaleEffectRange, _scaleEffect, 0.7f);
        }
        
        private float CalculateScaleEffect(float distance, float minDist, float maxDist, 
            float maxScale, float minScale)
        {
            float t = (distance - minDist) / (maxDist - minDist);
            float scale = Mathf.Lerp(maxScale, minScale, t);
    
            return scale;
        }
    }
}