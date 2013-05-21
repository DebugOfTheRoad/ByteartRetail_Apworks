﻿using ByteartRetail.DataObjects;
using ByteartRetail.Infrastructure.Communication;
using ByteartRetail.Infrastructure.Config;
using ByteartRetail.ServiceContracts;
using System;
using System.Web.Mvc;

namespace ByteartRetail.Web.MVC.Controllers
{
    [HandleError]
    public class LayoutController : ControllerBase
    {
        #region Shared Layout Partial View Actions
        public ActionResult CategoriesPartial()
        {
            using (var proxy = new ServiceProxy<IProductService>())
            {
                var categories = proxy.Channel.GetCategories(QuerySpec.Empty);
                return PartialView(categories);
            }
        }

        public ActionResult _LoginPartial()
        {
            if (User.Identity.IsAuthenticated)
            {
                using (var proxy = new ServiceProxy<IOrderService>())
                {
                    ViewBag.ShoppingCartItems = proxy.Channel.GetShoppingCartItemCount(UserID);
                }
            }
            return PartialView();
        }

        public ActionResult FeaturedProductsPartial()
        {
            using (var proxy = new ServiceProxy<IProductService>())
            {
                var featuredProducts = proxy.Channel.GetFeaturedProducts(4);
                return PartialView(featuredProducts);
            }
        }

        public ActionResult ProductsPartial(string categoryID = null, bool? fromIndexPage = null, int pageNumber = 1)
        {
            using (var proxy = new ServiceProxy<IProductService>())
            {
                var numberOfProductsPerPage = ByteartRetailConfigurationReader.Instance.ProductsPerPage;
                var pagination = new Pagination { PageSize = numberOfProductsPerPage, PageNumber = pageNumber };
                ProductDataObjectListWithPagination productsWithPagination = 
                    string.IsNullOrEmpty(categoryID) ? 
                    proxy.Channel.GetProductsWithPagination(pagination) : 
                    proxy.Channel.GetProductsForCategoryWithPagination(new Guid(categoryID), pagination);

                if (fromIndexPage != null &&
                    !fromIndexPage.Value)
                {
                    if (string.IsNullOrEmpty(categoryID))
                        ViewBag.CategoryName = "所有商品";
                    else
                    {
                        var category = proxy.Channel.GetCategoryByID(new Guid(categoryID), QuerySpec.Empty);
                        ViewBag.CategoryName = category.Name;
                    }
                }
                else
                    ViewBag.CategoryName = null;
                ViewBag.CategoryID = categoryID;
                ViewBag.FromIndexPage = fromIndexPage;
                if (fromIndexPage == null || fromIndexPage.Value)
                    ViewBag.Action = "Index";
                else
                    ViewBag.Action = "Category";
                ViewBag.IsFirstPage = productsWithPagination.Pagination.PageNumber == 1;
                ViewBag.IsLastPage = productsWithPagination.Pagination.PageNumber == productsWithPagination.Pagination.TotalPages;
                return PartialView(productsWithPagination);
            }
        }
        #endregion


    }
}
