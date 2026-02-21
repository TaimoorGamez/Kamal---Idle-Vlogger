using TMPro;
using UnityEngine;

namespace Core.Purchase
{
    public class ProductHandler : MonoBehaviour
    {
        [SerializeField] InAppPurchase InAppPurchaser;
        [SerializeField] StoreProduct CurrentProdut;
        [SerializeField] TextMeshProUGUI priceText;


        private void Start()
        {
            UpdatePriceValue();
        }
        
        private void UpdatePriceValue()
        {
            string currentPrice = InAppPurchaser.GetPrice(CurrentProdut.ProductID);
            if (currentPrice != " ")
            {
                priceText.text = currentPrice;
            }
        }

        public void OnClickBuyButton()
        {
            InAppPurchaser.BuyProduct(CurrentProdut.ProductID);
        }
    }
}
