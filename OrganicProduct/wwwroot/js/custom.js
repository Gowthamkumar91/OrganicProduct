document.addEventListener("DOMContentLoaded", function () {
    //  Swiper
    var swiper = new Swiper('.swiper', {
        slidesPerView: 3,
        spaceBetween: 20,
        loop: true,
        autoplay: {
            delay: 3000,
        },
        breakpoints: {
            768: { slidesPerView: 3 },
            576: { slidesPerView: 2 },
            0: { slidesPerView: 1 }
        }
    });

    //  Alert auto-close
    setTimeout(function () {
        const alert = document.querySelector('.alert');
        if (alert) alert.style.display = 'none';
    }, 3000);

    //  Sidebar toggle
    $('#sidebarToggle').click(function () {
        $('.admin-sidebar').toggleClass('collapsed');
        $('.admin-main-content').toggleClass('expanded');
    });

    //  Dark Mode Handling (ONLY ONCE)
    const themeToggle = document.getElementById("themeToggle");
    const savedTheme = localStorage.getItem("theme");

    if (savedTheme === "dark") {
        document.body.classList.add("dark-mode");
        if (themeToggle) themeToggle.checked = true;
    }

    if (themeToggle) {
        themeToggle.addEventListener("change", function () {
            const isDark = this.checked;
            document.body.classList.toggle("dark-mode", isDark);
            localStorage.setItem("theme", isDark ? "dark" : "light");
        });
    }

    //  Search & filter (admin)
    const searchInput = document.getElementById("searchInput");
    const statusFilter = document.getElementById("statusFilter");
    const tableBody = document.getElementById("ordersTable")?.getElementsByTagName("tbody")[0];

    if (searchInput && statusFilter && tableBody) {
        function filterOrders() {
            const search = searchInput.value.toLowerCase();
            const status = statusFilter.value;

            Array.from(tableBody.rows).forEach(row => {
                const customer = row.cells[2].textContent.toLowerCase();
                const orderStatus = row.cells[5].textContent;
                const matchSearch = customer.includes(search);
                const matchStatus = !status || orderStatus === status;

                row.style.display = (matchSearch && matchStatus) ? "" : "none";
            });
        }

        searchInput.addEventListener("keyup", filterOrders);
        statusFilter.addEventListener("change", filterOrders);
    }

    //  Login Page - Hide items for Admin
    setTimeout(function () {
        const emailInput = document.querySelector("input[name='Email']");
        const forgotLink = document.querySelector("a[href*='ForgotPassword']");
        const rememberCheckbox = document.querySelector("input[name='RememberMe']");
        const signUpLink = document.querySelector("a.text-primary");
        const signUpParagraph = signUpLink ? signUpLink.closest("p") || signUpLink.parentElement : null;

        if (!emailInput || !forgotLink || !rememberCheckbox || !signUpLink || !signUpParagraph) {
            console.warn("Some elements missing.");
            return;
        }

        emailInput.addEventListener("input", function () {
            const email = emailInput.value.trim().toLowerCase();
            const isAdmin = email === "organicadmin@gmail.com";

            forgotLink.style.display = isAdmin ? "none" : "inline";

            const rememberFormCheck = rememberCheckbox.closest(".form-check");
            if (rememberFormCheck) rememberFormCheck.style.display = isAdmin ? "none" : "block";

            signUpParagraph.style.display = isAdmin ? "none" : "block";
        });

        emailInput.dispatchEvent(new Event("input")); // trigger on load
    }, 300);

    //  Guest Add to Cart Handling
    const pendingId = localStorage.getItem("pendingCartProductId");
    if (pendingId) {
        localStorage.removeItem("pendingCartProductId");
        console.log("Redirecting to Cart after login for product:", pendingId);

        window.location.href = `/Cart/AddToCartRedirect?productId=${pendingId}`;
    }
});

//  AddToCart alert function
function handleGuestCart(productId) {
    alert("Please login to add items to cart.");
    localStorage.setItem("pendingCartProductId", productId);
    console.log("Storing productId before login:", productId);

    window.location.href = "/Account/Login";
}
