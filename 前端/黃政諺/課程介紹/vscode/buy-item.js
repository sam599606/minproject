localStorage.clear();
products = document.querySelectorAll('.item-product');
for(var i = 0; i < products.length; i++) {
    (function(index) {
        products[index].addEventListener('click', function() {
            var name = products[index].querySelector('.item-product-name').textContent;
            var url = products[index].querySelector('.item-product-img').getAttribute('src');
            var cart = [
                {
                    name: name,
                    price: 1000,
                    url: url,
                    dueDate: '6/24'
                }
                ];
            localStorage.setItem('cart', JSON.stringify(cart));
        });
    })(i);
}