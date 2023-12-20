document.addEventListener("DOMContentLoaded", (e) => {
    function updateCountInCart() {
        fetch("/Cart/Index?handler=OrdersCount", {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            },
        })
            .then(response => {
                if (response.ok) {
                    return response.json();
                }
                throw new Error(`Request failed with status ${response.status}`);
            })
            .then(data => {
                const cartBadgeCount = document.getElementById("cart-badge-count");
                if (cartBadgeCount) {
                    cartBadgeCount.innerText = data.count;
                }
            })
            .catch(error => {
                console.error(error);
            });
    }

    updateCountInCart()

    setInterval(updateCountInCart, 1000);
});