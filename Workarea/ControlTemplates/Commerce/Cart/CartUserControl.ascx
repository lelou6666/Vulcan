<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CartUserControl.ascx.cs" Inherits="CartUserControl" %>
<link id="EktronECommerceCartCSS" href="../../../csslib/Commerce/Cart.css" rel="stylesheet" type="text/css" />
<asp:literal id="DesignTimeWrapper" runat="server" ><link id="EktronECommerceCartCSS" href="../../../csslib/Commerce/Cart.css" type="text/css" rel="stylesheet" /></asp:literal>
<div id="EktronCartControl" class="ektron EktronCartWrapper" runat="server" >
    <div class="EktronCartCtl_cartContainer">
        <table class="cartTable" cellspacing="0">
            <caption id="Text_YourShoppingCartString" runat="server" >Your Cart</caption>
            <thead>
                <tr class="rowCartData">
                    <th colspan="2" class="cartName alignLeft">
                        <span class="label"><span id="Text_CartLabelString" runat="server" >Cart:</span></span>
                        <a id="Link_RenameCart" class="button buttonInline greenHover renameCart" href="#" runat="server" >
                            <span id="Text_ForTextIn_Link_RenameCart" runat="server" >Cart Two</span>
                            <img id="Image_RenameCart" src="../../../images/application/commerce/renameCart.gif" alt="Rename Cart" runat="server" /></a>
                        <div id="RenameCartUI" class="divButton" style="display: none;" runat="server" >
                            <asp:TextBox id="TextBox_RenameCart" runat="server" />
                            <asp:ImageButton id="Image_RenameCartOK" ImageUrl="../../../images/application/commerce/renameBasketOK.gif" runat="server" />
                            <img id="Image_RenameCartCancel" src="../../../images/application/commerce/renameBasketCancel.gif" alt="Cancel" runat="server" />
                        </div>
                    </th>
                    <th class="cartActions alignRight" colspan="5">
                        <div class="cartActionsListWrapper">
                            <ul class="cartActionsList">
                                <li>
                                <asp:LinkButton id="Link_RemoveAllCartItems" class="button buttonLeft deleteBasketButton redHover" runat="server" >
                                        <img id="Image_DeleteCart_1" src="../../../images/application/commerce/deleteCart.gif" alt="Empty Cart" runat="server" />
                                        <span id="Text_ForTextIn_Link_RemoveAllCartItems" runat="server" >Empty Cart</span>
                                        </asp:LinkButton></li>
                            </ul>
                        </div>
                    </th>
                </tr>
                <tr class="rowColumnHeadings">
                    <th><span id="Text_ForCurrentCart_ItemColumn" runat="server" >Item</span></th>
                    <th><span id="Text_ForCurrentCart_RemoveColumn" runat="server" >Remove</span></th>
                    <th><span id="Text_ForCurrentCart_SkuColumn" runat="server" >SKU</span></th>
                    <th><span id="Text_ForCurrentCart_QuantityColumn" runat="server" >Quantity</span></th>
                    <th><span id="Text_ForCurrentCart_ListPriceColumn" runat="server" >List Price</span></th>
                    <th><span id="Text_ForCurrentCart_SalePriceColumn" runat="server" >Sale Price</span></th>
                    <th class="colTotalHeader"><span id="Text_ForCurrentCart_TotalColumn" runat="server" >Total</span></th>
                </tr>
            </thead>
            <tbody id="CurrentCartTableBody" runat="server">
                <tr class="rowSku stripe">
                    <td class="colItemName">
                        <a href="_testProduct.aspx?id=43">Atrium Lounge Chair</a></td>
                    <td class="colRemove">
                        <a class="removeFromCart" href="#" >
                            <img id="Image_RemoveFromCart_1" src="../../../images/application/commerce/removefromcart2.gif" alt="Remove From Cart" runat="server" /></a></td>
                    <td class="colProductId">
                        atrium-chair</td>
                    <td class="colQty">
                        <input class="productQtyText" type="text" value="1" /></td>
                    <td class="colItemPrice">
                        $399.00</td>
                    <td class="colEarlyPrice">
                        $300.00</td>
                    <td class="colTotal ieBorderFix">
                        $300.00</td>
                </tr>
                <tr class="rowSku">
                    <td colspan="6" class="colDiscountCoupon">
                        Coupon Code:<span class="couponCode">tc1</span>, Discount:</td>
                    <td class="colTotal ieBorderFix">
                        <span class="couponDiscountAmount">$25.00</span><a class="removeCoupon" href="#" >
                        <img id="Image_RemoveFromCart_2" src="../../../images/application/commerce/removefromcart2.gif" alt="Remove Coupon" runat="server" /></a></td>
                </tr>
            </tbody>
            <tfoot>
                <tr class="rowSubtotal">
                    <td colspan="6" class="noBorderTopBottom alignRight">
                        <span id="Text_ForTextIn_SubtotalLabel" runat="server" >Subtotal:</span></td>
                    <td class="noBackgroundImage ieBorderFix">
                        <span id="Text_ForTextIn_Subtotal" runat="server" >$275.00</span></td>
                </tr>
                <tr id="AdvisoryMessageContainer" class="rowAdvisoryMessage" visible="false" runat="server">
                    <td colspan="7" class="noBorderTopBottom">
                        <span id="Text_ForTextIn_AdvisoryMessage" runat="server" ></span></td>
                </tr>
                <tr class="rowContinueShopping">
                    <td class="continueShopping" colspan="1" style="height: 79px">
                        <a id="Link_ContinueShopping_1" class="button buttonLeft blueHover" href="#" runat="server" >
                            <img id="Image_ContinueShopping_1" src="../../../images/application/commerce/continueShopping.gif" alt="Continue Shopping" runat="server" />
                            <span id="Text_ForTextIn_Link_ContinueShopping_1" runat="server" >Continue Shopping</span>
                        </a>
                    </td>
                    <td colspan="2">
                        <span id="EmptyCartNotice" visible="false" class="emptyCart" runat="server">The cart is empty</span>
                    </td>
                    <td class="checkout alignRight" colspan="4" style="height: 79px">
                        <div class="checkoutActionsWrapper" id="CheckoutActionsWrapper" runat="server">
                            <ul class="checkoutActionsList">
                                <li>
                                    <a id="Link_Checkout" class="button buttonRight checkoutCartButton greenHover" href="#" runat="server" >
                                        <img id="Image_Checkout" src="../../../images/application/commerce/checkout.gif" alt="Checkout" runat="server" />
                                        <span id="Text_ForTextIn_Link_Checkout" runat="server" >Checkout</span></a></li>
                                <li>
                                    <a id="Link_ApplyCouponButton" class="button buttonRight applyCartButton blueHover ApplyCouponButton" href="#" runat="server" >
                                        <img id="Image_Coupon" src="../../../images/application/commerce/coupon.gif" alt="Apply Coupon" runat="server" />
                                        <span id="Text_ForTextIn_Link_ApplyCouponButton" runat="server" >Apply Coupon</span></a></li>
                                <li>
                                    <div id="ApplyCouponUI" style="display: none;" runat="server" class="ApplyCouponUI">
                                        <span id="Text_ForEnterCouponCode" runat="server" >Enter Coupon Code:</span>
                                        <asp:TextBox id="TextBox_ApplyCoupon" class="ApplyCouponField" runat="server" />
                                        <asp:ImageButton id="Image_ApplyCouponOk" ImageUrl="../../../images/application/commerce/renameBasketOK.gif" ToolTip="Ok" runat="server" />
                                        <img id="Image_ApplyCouponCancel" src="../../../images/application/commerce/renameBasketCancel.gif" alt="Cancel" runat="server" />
                                    </div>
                                </li>
                                <li>
                                    <asp:LinkButton id="Link_UpdateSubtotal" class="button buttonRight updateCartButton blueHover" runat="server" >
                                        <img id="Image_UpdateSubtotal" src="../../../images/application/commerce/updateSubtotal.gif" alt="Update Subtotal" runat="server" />
                                        <span id="Text_ForTextIn_Link_UpdateSubtotal" runat="server" >Update Subtotal</span></asp:LinkButton></li>
                            </ul>
                        </div>
                    </td>
                </tr>
            </tfoot>
        </table>
    </div>
    <div class="EktronCartCtl_savedCartContainer" id="SavedCartContainer" runat="server">
        <table class="savedCarts" summary="Saved Carts">
            <caption id="Text_YourSavedCartsString" runat="server" >Your Saved Carts</caption>
            <thead>
                <tr>
                    <th><span id="Text_ForSavedCarts_CartColumn" runat="server" >Cart</span></th>
                    <th><span id="Text_ForSavedCarts_LastUpdatedColumn" runat="server" >Last Updated</span></th>
                    <th><span id="Text_ForSavedCarts_ItemsColumn" runat="server" >Items</span></th>
                    <th><span id="Text_ForSavedCarts_SubtotalColumn" runat="server" >Subtotal</span></th>
                    <th><span id="Text_ForSavedCarts_DeleteColumn" runat="server" >Delete</span></th>
                </tr>
            </thead>
            <tbody id="SavedCartTableBody" runat="server">
                <tr class="stripe">
                    <td class="colCartName">
                        <a href="#" >
                            Cart One</a></td>
                    <td class="colLastModified">
                        10/21/2008 7:49:56 PM</td>
                    <td class="colCartItems">
                        2</td>
                    <td class="colcartSubtotal">
                        $750.00</td>
                    <td class="colDeleteSavedCart">
                        <a class="button buttonRight" href="#" >
                            <img id="Image_DeleteCart_2" src="../../../images/application/commerce/deleteCart.gif" alt="Delete Cart" runat="server" /></a></td>
                </tr>
                <tr class="savedCart">
                    <td class="colCartName">
                        <a href="#" >
                            Cart Two - Active Cart</a></td>
                    <td class="colLastModified">
                        10/21/2008 6:10:39 PM</td>
                    <td class="colCartItems">
                        1</td>
                    <td class="colcartSubtotal">
                        $300.00</td>
                    <td class="colDeleteSavedCart">
                    </td>
                </tr>
                <tr class="stripe">
                    <td class="colCartName">
                        <a href="#" >
                            tc7a</a></td>
                    <td class="colLastModified">
                        10/21/2008 8:05:22 PM</td>
                    <td class="colCartItems">
                        3</td>
                    <td class="colcartSubtotal">
                        $850.00</td>
                    <td class="colDeleteSavedCart">
                        <a class="button buttonRight" href="#" >
                            <img id="Image_DeleteCart_3" src="../../../images/application/commerce/deleteCart.gif" alt="Delete Cart" runat="server" />
                        </a></td>
                </tr>
            </tbody>
            <tfoot>
                <tr class="rowContinueShopping">
                    <td>
                        &nbsp;<a id="Link_ContinueShopping_2" class="button buttonLeft blueHover" href="#" runat="server" ><img id="Image_ContinueShopping_2" src="../../../images/application/commerce/continueShopping.gif" alt="Continue Shopping" runat="server" />
                            <span id="Text_ForTextIn_Link_ContinueShopping_2" runat="server" >Continue Shopping</span></a></td>
                    <td colspan="4">
                        <a id="Link_CreateNewCart" class="button buttonRight greenHover CreateCartButton" href="#" runat="server" >
                            <img id="Image_CreateNewCart" src="../../../images/application/commerce/createNewBasket.gif" alt="Create New Cart" runat="server" />
                            <span id="Text_ForTextIn_Link_CreateNewCart" runat="server" >Create New Cart</span>
                        </a>
                        <div id="CreateCartUI" class="divButton buttonRight" style="display: none;" runat="server" >
                            <asp:TextBox id="TextBox_CreateCart" runat="server" />
                            <asp:ImageButton id="Image_CreateCartOk" ImageUrl="../../../images/application/commerce/renameBasketOK.gif"
                                ToolTip="Ok" runat="server" />
                            <img id="Image_CreateCartCancel" src="../../../images/application/commerce/renameBasketCancel.gif" alt="Cancel" runat="server" />
                        </div>
                    </td>
                </tr>
            </tfoot>
        </table>
    </div>
    <asp:HiddenField ID="ActionCode" Value="" runat="server" />
    <asp:HiddenField ID="ActionArgument" Value="" runat="server" />
</div>
