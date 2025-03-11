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
            url: "/Home/Login",  // Call the Login action in HomeController
            type: "GET",
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

document.querySelector('.form-range').addEventListener('input', function () {
    document.getElementById('priceValue').innerText = '₹' + this.value;
});