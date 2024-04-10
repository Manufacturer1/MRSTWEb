function updateTotalPrice() {
    var totalPrice = 0.0;
    $('.product').each(function () {
        var quantityInput = $(this).find('.quantity-input');
        var priceText = $(this).find('.price').text().replace('$', '');

        if (quantityInput.val() !== "" && !isNaN(quantityInput.val())) {
            var quantity = parseInt(quantityInput.val());
            var price = parseFloat(priceText.replace(',', '.'));
            totalPrice += quantity * price;
        }
    });

    $('.summary-item .price').text('$' + totalPrice.toFixed(2));
}
$(document).ready(function () {
    updateTotalPrice();
    displayCartItemCount();
 
});
function displayCartItemCount() {
    var cartItemCount = localStorage.getItem('cartItemCount') | 0;

    if (cartItemCount !== null && cartItemCount !== undefined) {
        $('#cartItemCount').text(cartItemCount);
    } else {
        cartItemCount = 0;
    }
   
    $('#cartItemCount').text(cartItemCount);
}
function updateCartItemCount(action) {
    var cartItemCount = localStorage.getItem("cartItemCount") | 0;

   
    cartItemCount = parseInt(cartItemCount);

    if (action === 'add') {
        cartItemCount++;
    } else if (action === 'remove') {
        cartItemCount--;
     
        if (cartItemCount < 0) {
            cartItemCount = 0;
        }
    }
    console.log(cartItemCount);
    $('#cartItemCount').text(cartItemCount);

    localStorage.setItem('cartItemCount', cartItemCount);
}



function updateTotalPriceAndItemCount(action) {
    updateTotalPrice(); 
    updateCartItemCount(action);
}
