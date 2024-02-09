using System;
using Jrd.Gameplay.Products;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel.Part
{
    public class ProductionBox : IBcpProductionBox
    {
        protected VisualElement Container;
        protected Label NameLabel;
        protected VisualTreeAsset ItemContainerTemplate;

        protected string ContainerId;
        protected string NameLabelId;

        public void SetItems(NativeList<ProductData> productsData)
        {
            if (productsData.IsEmpty || productsData.Length == 0)
            {
                SetEmptyBox();
            }

            Debug.LogWarning("Set items " + this);
        }

        private void SetEmptyBox()
        {
            throw new NotImplementedException();
        }
    }

    public interface IBcpProductionBox
    {
        public void SetItems(NativeList<ProductData> productsData);
    }
}