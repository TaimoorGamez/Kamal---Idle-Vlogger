using System.Linq;
using UnityEngine;
using Core.Plugins;
using UnityEngine.Purchasing;
using System.Collections.Generic;

namespace Core.Purchase
{
    public class InAppPurchase : MonoBehaviour
    {
        [HideInInspector] public bool IsInitialized;

        [SerializeField] NonConsumableProduct[] NonConsumableProducts;
        //[SerializeField] ConsumableProduct[] ConsumableProducts;

        Dictionary<string , StoreProduct> productDictionary;
        StoreController m_StoreController;

        public void InitializePurchasing()
        {
            if (IsInitialized || !RemoteDataHolder.AdData.CanPurchase)
                return;

            InitializeIAP();
        }

        async void InitializeIAP()
        {
            m_StoreController = UnityIAPServices.StoreController();

            m_StoreController.OnPurchasePending += OnPurchasePending;
            m_StoreController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            m_StoreController.OnPurchaseFailed += OnPurchaseFailed;

            m_StoreController.OnStoreDisconnected += OnStoreDisconnected;
            //Debug.Log("Connecting to store.");
            await m_StoreController.Connect();

            m_StoreController.OnProductsFetchFailed += OnProductsFetchedFailed;
            m_StoreController.OnProductsFetched += OnProductsFetched;
            FetchProducts();
        }

        void FetchProducts()
        {
            IsInitialized = true;
            productDictionary = new Dictionary<string, StoreProduct>();
            List<ProductDefinition> initialProductsToFetch = new List<ProductDefinition>();
            for (int n = 0; n < NonConsumableProducts.Length; n++)
            {
                initialProductsToFetch.Add(new ProductDefinition(NonConsumableProducts[n].ProductID, ProductType.NonConsumable));
                productDictionary.Add(NonConsumableProducts[n].ProductID, NonConsumableProducts[n]);
            }
            //for (int c = 0; c < ConsumableProducts.Length; c++)
            //{
            //    initialProductsToFetch.Add(new ProductDefinition(ConsumableProducts[c].ProductID, ProductType.Consumable));
            //    productDictionary.Add(ConsumableProducts[c].ProductID, ConsumableProducts[c]);
            //}
            m_StoreController.FetchProducts(initialProductsToFetch);
        }

        public string GetPrice(string productID)
        {
            //Debug.Log("GetPrice called for productID: " + IsInitialized);
            if (IsInitialized)
            {
                Product product = m_StoreController.GetProductById(productID);
                return product.metadata.localizedPriceString;
            }
            else
            {
                return " ";
            }
        }

        public void BuyProduct(string productId)
        {
            if (IsInitialized)
                m_StoreController.PurchaseProduct(productId);
        }

        void OnPurchaseFailed(FailedOrder order)
        {
            var product = GetFirstProductInOrder(order);
            if (product == null)
            {
                Debug.Log("Could not find product in failed order.");
            }

            Debug.Log($"Purchase failed - Product: '{product?.definition.id}'," +
                      $"PurchaseFailureReason: {order.FailureReason.ToString()},"
                      + $"Purchase Failure Details: {order.Details}");
        }

        void OnPurchasePending(PendingOrder order)
        {
            Product product = GetFirstProductInOrder(order);
            if (product is null)
            {
                //Debug.Log("Could not find product in order.");
                return;
            }

            StoreProduct storeProduct = productDictionary[product.definition.id];
            storeProduct.BuyProduct();

            m_StoreController.ConfirmPurchase(order);
        }

        void OnPurchaseConfirmed(Order order)
        {
            switch (order)
            {
                case ConfirmedOrder confirmedOrder:
                    OnPurchaseConfirmed(confirmedOrder);
                    break;
                case FailedOrder failedOrder:
                    OnPurchaseConfirmationFailed(failedOrder);
                    break;
                default:
                    //Debug.Log("Unknown OnPurchaseConfirmed result.");
                    break;
            }
        }

        void OnPurchaseConfirmed(ConfirmedOrder order)
        {
            //Product product = GetFirstProductInOrder(order);
            //if (product == null)
            //{
            //    Debug.Log("Could not find product in purchase confirmation.");
            //}
            //Debug.Log($"Purchase confirmed- Product: {product?.definition.id}");
        }

        void OnPurchaseConfirmationFailed(FailedOrder order)
        {
            //Product product = GetFirstProductInOrder(order);
            //if (product == null)
            //{
            //    Debug.Log("Could not find product in failed confirmation.");
            //}

            //Debug.Log($"Confirmation failed - Product: '{product?.definition.id}'," +
            //          $"PurchaseFailureReason: {order.FailureReason.ToString()},"
            //          + $"Confirmation Failure Details: {order.Details}");
        }

        Product GetFirstProductInOrder(Order order)
        {
            return order.CartOrdered.Items().First()?.Product;
        }

        // Calling StoreController.Connect without a listener on the StoreController.OnStoreDisconnected event will result in warnings.
        void OnStoreDisconnected(StoreConnectionFailureDescription description)
        {
            //Debug.Log($"Store disconnected details: {description.message}");
        }

        // Calling StoreController.Connect without listeners on StoreController.OnProductsFetched and StoreController.OnProductsFetchedFailed will result in warnings.
        void OnProductsFetched(List<Product> products)
        {
            //Debug.Log($"Products fetched successfully for {products.Count} products.");
        }

        void OnProductsFetchedFailed(ProductFetchFailed failure)
        {
            //Debug.Log($"Products fetch failed for {failure.FailedFetchProducts.Count} products: {failure.FailureReason}");
        }

    }
}
