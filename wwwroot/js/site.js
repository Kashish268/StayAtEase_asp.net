// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



$(document).ready(function () {
    $("#openRegister").click(function (event) {
        event.preventDefault();
        $("#loginModal").modal("hide"); // Close Login Modal

        // Once login modal is fully hidden, open Register Modal
        $("#loginModal").one("hidden.bs.modal", function () {
            $("#registerModal").modal("show");
        });
    });

    // When "Login" is clicked inside the Register Modal
    $("#openLogin").click(function (event) {
        event.preventDefault();
        $("#registerModal").modal("hide"); // Close Register Modal

        // Once register modal is fully hidden, open Login Modal
        $("#registerModal").one("hidden.bs.modal", function () {
            $("#loginModal").modal("show");
        });
    });

    $(document).on("click", "#regSuccess", function (event) {
        event.preventDefault();

        // Hide Register Modal and open Login Modal after it's fully hidden
        $("#registerModal").modal("hide").one("hidden.bs.modal", function () {
            $("#loginModal").modal("show");
        });
    });



    // When "Close" is clicked in Register Modal, open Login Modal
    $("#registerModal").on("hidden.bs.modal", function (e) {
        if (!$(".modal:visible").length) {  // Check if no modal is currently open
            $("#loginModal").modal("show");
        }
    });

    // When "Close" is clicked in Login Modal, close everything
    $("#loginModal").on("hidden.bs.modal", function () {
        if (!$(".modal:visible").length) {  // Check if no modal is currently open
            $(".modal").modal("hide"); // Ensure all modals are closed
        }
    });


    $("#openLoginModal").click(function () {
        $.ajax({
            url: "/Account/Login",  // Call the Login action in HomeController
            type: "POST",
            success: function (data) {
                $("#loginContent").html(data); // Inject response into modal
                $("#loginModal").modal("show"); // Show the modal
            },
            error: function () {
                alert("Error loading login form.");
            }
        });
    });

    $("#openRegister").click(function (event) {
        event.preventDefault(); // Prevent default link behavior

        $.ajax({
            url: "/Home/Register",  // Calls Register action in HomeController
            type: "GET",
            success: function (data) {
                $("#registerContent").html(data); // Inject form into modal
                $("#registerModal").modal("show"); // Open the modal
            },
            error: function (xhr, status, error) {
                console.error("Error loading register form:", error);
                alert("Error loading register form.");
            }
        });
    });
});

//=========liked button================


document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".like-btn").forEach(button => {
        button.addEventListener("click", function () {
            this.classList.toggle("liked");
        });
    });
});

//============Filter button===============//

function toggleActive(button) {
    document.querySelectorAll('.filter-btn').forEach(btn => btn.classList.remove('active'));
    button.classList.add('active');
}

//============price====================//

//document.querySelector('.form-range').addEventListener('input', function () {
//    document.getElementById('priceValue').innerText = '₹' + this.value;
//});

//==========rating==================//

function setRating(rating) {
    let stars = document.querySelectorAll('.star');
    stars.forEach((star, index) => {
        if (index < rating) {
            star.classList.add('selected');
        } else {
            star.classList.remove('selected');
        }
    });
}


document.addEventListener("DOMContentLoaded", function () {
    const multipleItemCarousel = document.querySelector("#testimonialCarousel");

    if (window.matchMedia("(min-width: 576px)").matches) {
        const carousel = new bootstrap.Carousel(multipleItemCarousel, {
            interval: false,
            wrap: false
        });

        let carouselInner = document.querySelector(".carousel-inner");
        let carouselItems = document.querySelectorAll(".carousel-item");
        let cardWidth = carouselItems[0].offsetWidth;
        let scrollPosition = 0;

        document.querySelector(".carousel-control-next").addEventListener("click", function () {
            let maxScroll = carouselInner.scrollWidth - carouselInner.clientWidth;
            if (scrollPosition < maxScroll) {
                scrollPosition += cardWidth;
                carouselInner.scrollTo({ left: scrollPosition, behavior: "smooth" });
            }
        });

        document.querySelector(".carousel-control-prev").addEventListener("click", function () {
            if (scrollPosition > 0) {
                scrollPosition -= cardWidth;
                carouselInner.scrollTo({ left: scrollPosition, behavior: "smooth" });
            }
        });

    } else {
        multipleItemCarousel.classList.add("slide");
    }
});


//================================//


//================================================for super admin ==============================================
document.addEventListener("DOMContentLoaded", function () {
    // Shared Pagination Logic
    function setupPagination(data, rowsPerPage, containerId, infoId, renderFunction) {
        let currentPage = 1;

        function updatePagination() {
            const container = document.getElementById(containerId);
            const info = document.getElementById(infoId);
            container.innerHTML = "";

            const totalPages = Math.ceil(data.length / rowsPerPage);
            const start = (currentPage - 1) * rowsPerPage;
            const end = Math.min(start + rowsPerPage, data.length);

            if (info) {
                info.textContent = `Showing ${start + 1} to ${end} of ${data.length}`;
            }

            for (let i = 1; i <= totalPages; i++) {
                const pageBtn = document.createElement("button");
                pageBtn.textContent = i;
                pageBtn.className = `pagination-btn ${i === currentPage ? "active" : ""}`;
                pageBtn.addEventListener("click", function () {
                    currentPage = i;
                    renderFunction(currentPage, rowsPerPage);
                    updatePagination();
                });
                container.appendChild(pageBtn);
            }
        }

        // Initial call
        renderFunction(currentPage, rowsPerPage);
        updatePagination();
    }

    // ---------------------------------- USERS ----------------------------------
    if (typeof users !== "undefined") {
        function renderUsers(page, size) {
            const start = (page - 1) * size;
            const paginatedUsers = users.slice(start, start + size);
            // Render user table here...
            console.log("Rendering Users:", paginatedUsers);
        }
        setupPagination(users, 4, "pagination-buttons", "pagination-info", renderUsers);
    }

    // ---------------------------------- ADMINS ----------------------------------
    if (typeof admins !== "undefined") {
        function renderAdmins(page, size) {
            const start = (page - 1) * size;
            const paginatedAdmins = admins.slice(start, start + size);
            // Render admin table here...
            console.log("Rendering Admins:", paginatedAdmins);
        }
        setupPagination(admins, 4, "pagination-buttons", "pagination-info", renderAdmins);
    }

    // ---------------------------------- REVIEWS ----------------------------------
    if (typeof reviews !== "undefined") {
        function renderReviews(page, size) {
            const start = (page - 1) * size;
            const paginatedReviews = reviews.slice(start, start + size);
            // Render reviews here...
            console.log("Rendering Reviews:", paginatedReviews);
        }
        setupPagination(reviews, 6, "pagination-buttons", "pagination-info", renderReviews);
    }

    // ---------------------------------- MESSAGES / INQUIRIES ----------------------------------
    if (typeof messages !== "undefined") {
        function renderMessages(page, size) {
            const start = (page - 1) * size;
            const paginatedMessages = messages.slice(start, start + size);
            // Render messages/inquiries here...
            console.log("Rendering Messages:", paginatedMessages);
        }
        setupPagination(messages, 6, "pagination-buttons", "pagination-info", renderMessages);
    }

    // ---------------------------------- PROPERTIES ----------------------------------
    if (typeof properties !== "undefined") {
        function renderProperties(page, size) {
            const start = (page - 1) * size;
            const paginatedProps = properties.slice(start, start + size);
            // Render properties here...
            console.log("Rendering Properties:", paginatedProps);

            // Update stats
            document.getElementById("totalProperties").textContent = properties.length;
            document.getElementById("activeListings").textContent = properties.filter(p => p.status === "Available").length;
            document.getElementById("totalInquiries").textContent = properties.reduce((sum, p) => sum + (p.inquiries || 0), 0);
        }
        setupPagination(properties, 6, "pagination-buttons", "pagination-info", renderProperties);
    }
});




    loadProperties(); 
});



//==============FILETER==================//

 //document.addEventListener("DOMContentLoaded", function () {
 //       const searchInput = document.querySelector(".search-input");
 //       const propertyType = document.getElementById("propertyType");
 //       const priceRange = document.querySelector("input[type='range']");
 //       const priceValue = document.getElementById("priceValue");

 //       function filterCards() {
 //           const searchText = searchInput.value.toLowerCase();
 //           const selectedType = propertyType.value.toLowerCase();
 //           const maxPrice = parseInt(priceRange.value);

 //           const cards = document.querySelectorAll(".property-card"); // Re-query every time
 //           cards.forEach(card => {
 //               const title = card.dataset.title.toLowerCase();
 //               const location = card.dataset.location.toLowerCase();
 //               const type = card.dataset.type.toLowerCase();
 //               const price = parseInt(card.dataset.price);

 //               const matchesSearch = title.includes(searchText) || location.includes(searchText);
 //               const matchesType = selectedType === "🏠 property type" || type === selectedType;
 //               const matchesPrice = price <= maxPrice;

 //               if (matchesSearch && matchesType && matchesPrice) {
 //                   card.style.display = "block";
 //               } else {
 //                   card.style.display = "none";
 //               }
 //           });
 //       }

 //       searchInput.addEventListener("input", filterCards);
 //       propertyType.addEventListener("change", filterCards);
 //       priceRange.addEventListener("input", function () {
 //           priceValue.innerText = "₹" + priceRange.value;
 //           filterCards();
 //       });

 //       // Sorting
 //       document.querySelectorAll(".filter-btn").forEach(btn => {
 //           btn.addEventListener("click", function () {
 //               const action = btn.textContent.trim().toLowerCase();
 //               const container = document.querySelector(".row.g-4");
 //               const allCards = Array.from(document.querySelectorAll(".property-card"));

 //               // Show visible cards only
 //               const visibleCards = allCards.filter(card => card.style.display !== "none");

 //               if (action === "price ascending") {
 //                   visibleCards.sort((a, b) => parseInt(a.dataset.price) - parseInt(b.dataset.price));
 //               } else if (action === "price descending") {
 //                   visibleCards.sort((a, b) => parseInt(b.dataset.price) - parseInt(a.dataset.price));
 //               } else if (action === "new") {
 //                   visibleCards.sort((a, b) => new Date(b.dataset.date) - new Date(a.dataset.date));
 //               }

 //               // Re-append sorted visible cards
 //               visibleCards.forEach(card => container.appendChild(card));
 //           });
 //       });

 //       // Run initial filter on load
 //       filterCards();
 //   });


