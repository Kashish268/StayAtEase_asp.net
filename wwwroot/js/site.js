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

//--------------------------total - users----------------

document.addEventListener("DOMContentLoaded", function () {
    const users = [
        { id: 1, name: "User 1", email: "user1@example.com", mobile: "+1 888 888 1111", created: "01 Jan 2024", status: "Active", profilePic: "https://randomuser.me/api/portraits/men/1.jpg" },
        { id: 2, name: "User 2", email: "user2@example.com", mobile: "+1 888 888 2222", created: "02 Jan 2024", status: "Active", profilePic: "https://randomuser.me/api/portraits/women/2.jpg" },
        { id: 3, name: "User 3", email: "user3@example.com", mobile: "+1 888 888 3333", created: "03 Jan 2024", status: "Inactive", profilePic: "https://randomuser.me/api/portraits/men/3.jpg" },
        { id: 4, name: "User 4", email: "user4@example.com", mobile: "+1 888 888 4444", created: "04 Jan 2024", status: "Active", profilePic: "https://randomuser.me/api/portraits/women/4.jpg" },
        { id: 5, name: "User 5", email: "user5@example.com", mobile: "+1 888 888 5555", created: "05 Jan 2024", status: "Inactive", profilePic: "https://randomuser.me/api/portraits/men/5.jpg" },
        { id: 6, name: "User 6", email: "user6@example.com", mobile: "+1 888 888 6666", created: "06 Jan 2024", status: "Active", profilePic: "https://randomuser.me/api/portraits/women/6.jpg" },
        { id: 7, name: "User 7", email: "user7@example.com", mobile: "+1 888 888 7777", created: "07 Jan 2024", status: "Inactive", profilePic: "https://randomuser.me/api/portraits/men/7.jpg" },
        { id: 8, name: "User 8", email: "user8@example.com", mobile: "+1 888 888 8888", created: "08 Jan 2024", status: "Active", profilePic: "https://randomuser.me/api/portraits/women/8.jpg" },
        { id: 9, name: "User 9", email: "user9@example.com", mobile: "+1 888 888 9999", created: "09 Jan 2024", status: "Inactive", profilePic: "https://randomuser.me/api/portraits/men/9.jpg" },
        { id: 10, name: "User 10", email: "user10@example.com", mobile: "+1 888 888 0000", created: "10 Jan 2024", status: "Active", profilePic: "https://randomuser.me/api/portraits/women/10.jpg" }
    ];




    const pageSize = 4;
    let currentPage = 1;

    function renderTable() {
        const start = (currentPage - 1) * pageSize;
        const end = Math.min(start + pageSize, users.length);
        const tableBody = document.getElementById("userTableBody");
        tableBody.innerHTML = "";

        for (let i = start; i < end; i++) {
            const user = users[i];
            tableBody.innerHTML += `
                <tr>
                    <td>${user.id}</td>
                    <td>
                        <div class="total-users-info">
                            <img src="${user.profilePic}" alt="${user.name}" class="total-users-profile-pic">
                            <div>
                                <strong>${user.name}</strong>
                                <p>${user.email}</p>
                            </div>
                        </div>
                    </td>
                    <td>${user.mobile}</td>
                    <td>${user.created}</td>
                    <td><span class="total-users-status ${user.status.toLowerCase()}">${user.status}</span></td>
                    <td>
                        <button class="total-users-action-btn edit"><i class="fas fa-edit"></i></button>
                        <button class="total-users-action-btn delete"><i class="fas fa-trash"></i></button>
                    </td>
                </tr>
            `;
        }


        document.getElementById("pagination-info").textContent = `${start + 1} - ${end} of ${users.length} items`;
        renderPaginationButtons();
    }

    function renderPaginationButtons() {
        const paginationDiv = document.getElementById("pagination-buttons");
        paginationDiv.innerHTML = "";

        const totalPages = Math.ceil(users.length / pageSize);
        for (let i = 1; i <= totalPages; i++) {
            const button = document.createElement("button");
            button.textContent = i;
            button.className = `pagination-btn ${i === currentPage ? "active" : ""}`;
            button.onclick = function () {
                currentPage = i;
                renderTable();
            };
            paginationDiv.appendChild(button);
        }
    }

    renderTable();
});

//--------------------------total - admins----------------

document.addEventListener("DOMContentLoaded", function () {
    const admins = [
        { id: 1, name: "Admin 1", email: "admin1@example.com", mobile: "+1 888 888 1111", created: "01 Jan 2024", status: "Active", profilePic: "https://randomuser.me/api/portraits/men/1.jpg" },
        { id: 2, name: "Admin 2", email: "admin2@example.com", mobile: "+1 888 888 2222", created: "02 Jan 2024", status: "Active", profilePic: "https://randomuser.me/api/portraits/women/2.jpg" },
        { id: 3, name: "Admin 3", email: "admin3@example.com", mobile: "+1 888 888 3333", created: "03 Jan 2024", status: "Inactive", profilePic: "https://randomuser.me/api/portraits/men/3.jpg" },
        { id: 4, name: "Admin 4", email: "admin4@example.com", mobile: "+1 888 888 4444", created: "04 Jan 2024", status: "Active", profilePic: "https://randomuser.me/api/portraits/women/4.jpg" },
        { id: 5, name: "Admin 5", email: "admin5@example.com", mobile: "+1 888 888 5555", created: "05 Jan 2024", status: "Inactive", profilePic: "https://randomuser.me/api/portraits/men/5.jpg" },
        { id: 6, name: "Admin 6", email: "admin6@example.com", mobile: "+1 888 888 6666", created: "06 Jan 2024", status: "Active", profilePic: "https://randomuser.me/api/portraits/women/6.jpg" },
        { id: 7, name: "Admin 7", email: "admin7@example.com", mobile: "+1 888 888 7777", created: "07 Jan 2024", status: "Inactive", profilePic: "https://randomuser.me/api/portraits/men/7.jpg" },
        { id: 8, name: "Admin 8", email: "admin8@example.com", mobile: "+1 888 888 8888", created: "08 Jan 2024", status: "Active", profilePic: "https://randomuser.me/api/portraits/women/8.jpg" },
        { id: 9, name: "Admin 9", email: "admin9@example.com", mobile: "+1 888 888 9999", created: "09 Jan 2024", status: "Inactive", profilePic: "https://randomuser.me/api/portraits/men/9.jpg" },
        { id: 10, name: "Admin 10", email: "admin10@example.com", mobile: "+1 888 888 0000", created: "10 Jan 2024", status: "Active", profilePic: "https://randomuser.me/api/portraits/women/10.jpg" }
    ];



    const pageSize = 4;
    let currentPage = 1;

    function renderTable() {
        const start = (currentPage - 1) * pageSize;
        const end = Math.min(start + pageSize, admins.length);
        const tableBody = document.getElementById("adminTableBody");
        tableBody.innerHTML = "";

        for (let i = start; i < end; i++) {
            const admin = admins[i];
            tableBody.innerHTML += `
                <tr>
                    <td>${admin.id}</td>
                    <td>
                        <div class="total-users-info">
                            <img src="${admin.profilePic}" alt="${admin.name}" class="total-users-profile-pic">
                            <div>
                                <strong>${admin.name}</strong>
                                <p>${admin.email}</p>
                            </div>
                        </div>
                    </td>
                    <td>${admin.mobile}</td>
                    <td>${admin.created}</td>
                    <td><span class="total-users-status ${admin.status.toLowerCase()}">${admin.status}</span></td>
                    <td>
                        <button class="total-users-action-btn edit"><i class="fas fa-edit"></i></button>
                        <button class="total-users-action-btn delete"><i class="fas fa-trash"></i></button>
                    </td>
                </tr>
            `;
        }

        document.getElementById("pagination-info").textContent = `${start + 1} - ${end} of ${admins.length} items`;
        renderPaginationButtons();
    }

    function renderPaginationButtons() {
        const paginationDiv = document.getElementById("pagination-buttons");
        paginationDiv.innerHTML = "";

        const totalPages = Math.ceil(admins.length / pageSize);
        for (let i = 1; i <= totalPages; i++) {
            const button = document.createElement("button");
            button.textContent = i;
            button.className = `pagination-btn ${i === currentPage ? "active" : ""}`;
            button.onclick = function () {
                currentPage = i;
                renderTable();
            };
            paginationDiv.appendChild(button);
        }
    }

    renderTable();
});


//---------------------------- property reviews ---------------------------

document.addEventListener("DOMContentLoaded", function () {
    const reviews = [
        { propertyId: 101, title: "Luxury Apartment", reviewer: "John Smith", date: "Feb 15, 2024", rating: 5, review: "Great property with excellent amenities.", profilePic: "https://randomuser.me/api/portraits/men/1.jpg" },
        { propertyId: 102, title: "Cozy Condo", reviewer: "Emma Wilson", date: "Feb 14, 2024", rating: 5, review: "Absolutely love living here!", profilePic: "https://randomuser.me/api/portraits/women/2.jpg" },
        { propertyId: 103, title: "Spacious Home", reviewer: "Michael Brown", date: "Feb 13, 2024", rating: 4, review: "Good value for money.", profilePic: "https://randomuser.me/api/portraits/men/3.jpg" },
        { propertyId: 104, title: "Modern Studio", reviewer: "Sarah Davis", date: "Feb 12, 2024", rating: 5, review: "Beautiful property with great views.", profilePic: "https://randomuser.me/api/portraits/women/4.jpg" },
        { propertyId: 105, title: "Green Villas", reviewer: "Robert Johnson", date: "Feb 11, 2024", rating: 3, review: "Decent place, could improve on noise insulation.", profilePic: "https://randomuser.me/api/portraits/men/5.jpg" },
        { propertyId: 106, title: "Skyline Residences", reviewer: "Lisa Anderson", date: "Feb 10, 2024", rating: 5, review: "Exceptional property management!", profilePic: "https://randomuser.me/api/portraits/women/6.jpg" },
        { propertyId: 107, title: "Seaside Apartments", reviewer: "David Wilson", date: "Feb 9, 2024", rating: 5, review: "Great community events and friendly neighbors.", profilePic: "https://randomuser.me/api/portraits/men/7.jpg" },
        { propertyId: 108, title: "The Grand Suites", reviewer: "Jennifer Taylor", date: "Feb 8, 2024", rating: 5, review: "Modern apartments with great features.", profilePic: "https://randomuser.me/api/portraits/women/8.jpg" },
        { propertyId: 109, title: "Hilltop Haven", reviewer: "James Martinez", date: "Feb 7, 2024", rating: 4, review: "Good location but parking can be a challenge.", profilePic: "https://randomuser.me/api/portraits/men/9.jpg" },
        { propertyId: 110, title: "Urban Oasis", reviewer: "Patricia Thomas", date: "Feb 6, 2024", rating: 5, review: "Excellent property! Friendly staff and well-maintained.", profilePic: "https://randomuser.me/api/portraits/women/10.jpg" }
    ];

    const tableBody = document.getElementById("reviewsTableBody");
    const paginationButtons = document.getElementById("pagination-buttons");
    const paginationInfo = document.getElementById("pagination-info");

    let currentPage = 1;
    const rowsPerPage = 6;

    function displayReviews() {
        tableBody.innerHTML = "";

        const start = (currentPage - 1) * rowsPerPage;
        const end = start + rowsPerPage;
        const paginatedReviews = reviews.slice(start, end);

        paginatedReviews.forEach(review => {
            const row = document.createElement("tr");

            row.innerHTML = `
                <td>${review.propertyId}</td>
                <td>${review.title}</td>
                <td class="total-users-info">
                    <img src="${review.profilePic}" alt="${review.reviewer}" class="total-users-profile-pic">
                    ${review.reviewer}
                </td>
                <td>${review.date}</td>
                <td>${generateStars(review.rating)}</td>
                <td>${review.review}</td>
                <td><button class="total-users-action-btn delete"><i class="fas fa-trash"></i></button></td>
            `;

            tableBody.appendChild(row);
        });

        updatePagination();
    }

    function updatePagination() {
        paginationButtons.innerHTML = "";
        const totalPages = Math.ceil(reviews.length / rowsPerPage);

        paginationInfo.innerText = `Showing ${Math.min(reviews.length, currentPage * rowsPerPage)} of ${reviews.length} reviews`;

        for (let i = 1; i <= totalPages; i++) {
            const pageButton = document.createElement("button");
            pageButton.innerText = i;
            pageButton.className = `pagination-btn ${i === currentPage ? "active" : ""}`;
            pageButton.addEventListener("click", function () {
                currentPage = i;
                displayReviews();
            });
            paginationButtons.appendChild(pageButton);
        }
    }

    function generateStars(rating) {
        let starsHTML = "";
        for (let i = 1; i <= 5; i++) {
            if (i <= rating) {
                starsHTML += `<i class="fas fa-star"style="color:gold" ></i>`;  // Filled star
            } else {
                starsHTML += `<i class="far fa-star"></i>`;  // Empty star
            }
        }
        return starsHTML;
    }


    displayReviews();
});



//-------------------------------- inquires -----------------------------


document.addEventListener("DOMContentLoaded", function () {
    const messageTableBody = document.getElementById("messageTableBody");
    const paginationButtons = document.getElementById("pagination-buttons");
    const paginationInfo = document.getElementById("pagination-info");

    // Sample Data - Replace with API Fetch if needed
    const messages = [
        { guest: "Sarah Johnson", propertyId: "P_107", email: "sarahjohnson@gmail.com", contact: "1236547890", message: "Are there any restrictions on lease agreements?" },
        { guest: "Michael Brown", propertyId: "P_109", email: "michaelbrown@gmail.com", contact: "9874563210", message: "Are there any restrictions on lease agreements?" },
        { guest: "Emma Davis", propertyId: "P_110", email: "emmadavis@gmail.com", contact: "7410258963", message: "What documents are required for booking?" },
        { guest: "Michael Brown", propertyId: "P_105", email: "michaelbrown@gmail.com", contact: "9874563210", message: "Are there any restrictions on lease agreements?" },
        { guest: "Sarah Johnson", propertyId: "P_101", email: "sarahjohnson@gmail.com", contact: "1236547890", message: "Are there any restrictions on lease agreements?" },
        { guest: "Michael Brown", propertyId: "P_102", email: "michaelbrown@gmail.com", contact: "9874563210", message: "Are there any restrictions on lease agreements?" },
        { guest: "Emma Davis", propertyId: "P_103", email: "emmadavis@gmail.com", contact: "7410258963", message: "What documents are required for booking?" },
        { guest: "Michael Brown", propertyId: "P_105", email: "michaelbrown@gmail.com", contact: "9874563210", message: "Are there any restrictions on lease agreements?" },
        { guest: "Sarah Johnson", propertyId: "P_101", email: "sarahjohnson@gmail.com", contact: "1236547890", message: "Are there any restrictions on lease agreements?" },
        { guest: "Michael Brown", propertyId: "P_102", email: "michaelbrown@gmail.com", contact: "9874563210", message: "Are there any restrictions on lease agreements?" }
    ];

    let currentPage = 1;
    const rowsPerPage = 6;

    function loadMessages() {
        messageTableBody.innerHTML = ""; // Clear existing rows

        let start = (currentPage - 1) * rowsPerPage;
        let end = start + rowsPerPage;
        let paginatedItems = messages.slice(start, end);

        paginatedItems.forEach((msg, index) => {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${msg.guest}</td>
                <td>${msg.propertyId}</td>
                <td>${msg.email}</td>
                <td>${msg.contact}</td>
                <td>${msg.message}</td>
                <td>
                    <button class="total-users-action-btn delete"><i class="fas fa-trash"></i></button>                  
                </td>
            `;
            messageTableBody.appendChild(row);
        });

        paginationInfo.textContent = `Showing ${start + 1} to ${Math.min(end, messages.length)} of ${messages.length} messages`

      updatePagination();
    }

    function updatePagination() {
        paginationButtons.innerHTML = "";
        const totalPages = Math.ceil(messages.length / rowsPerPage);

        paginationInfo.innerText = `Showing ${Math.min(messages.length, currentPage * rowsPerPage)} of ${messages.length} reviews`;

        for (let i = 1; i <= totalPages; i++) {
            const pageButton = document.createElement("button");
            pageButton.innerText = i;
            pageButton.className = `pagination-btn ${i === currentPage ? "active" : ""}`;
            pageButton.addEventListener("click", function () {
                currentPage = i;
                loadMessages();
            });
            paginationButtons.appendChild(pageButton);
        }
    }


    loadMessages();
});