using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utility.MVC;

namespace Game.UI.Lobby.Tabs.Levels
{
    public class LevelsView : View
    {
        [Space, Header("Scroll Levels")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private LevelItemView levelPrefab;
        [SerializeField] private Transform levelContainer;

        [Space, Header("Settings")]
        [SerializeField] private int amountShowingLevels = 10;
        [SerializeField] private float shiftScroll = 0.12f;
        [SerializeField] private float durationHideElement = 0.4f;
        [SerializeField] private float delayBeforeAnimation = 2f;

        private readonly List<LevelItemView> items = new();

        public IReadOnlyList<LevelItemView> Items => items;
        public int AmountShowingLevels => amountShowingLevels;

        public LevelItemView CreateItem()
        {
            LevelItemView item = Instantiate(levelPrefab, levelContainer);

            items.Add(item);

            return item;
        }

        public void Clear()
        {
            foreach (LevelItemView item in items)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }

            items.Clear();
        }

        public void SetScrollEnabled(bool value)
        {
            if (!value)
                scrollRect.StopMovement();

            scrollRect.vertical = value;
        }

        public void ResetScroll() =>
            scrollRect.normalizedPosition = Vector2.zero;

        public Sequence CreateSwapSequence()
        {
            Sequence sequence = DOTween.Sequence();

            sequence.AppendInterval(delayBeforeAnimation);
            sequence.AppendCallback(() => items[0].PlayHide());
            sequence.Append(scrollRect.DOVerticalNormalizedPos(shiftScroll, durationHideElement));

            sequence.AppendCallback(() =>
            {
                LevelItemView next = items[1];

                next.SetState(LevelItemState.Open);
                next.PlayOpen();
            });

            return sequence;
        }
    }
}
