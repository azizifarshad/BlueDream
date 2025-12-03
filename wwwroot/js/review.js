$(document).ready(function () {

    // Submit new review
    $("#reviewForm").on("submit", function (e) {
        e.preventDefault();

        $.ajax({
            url: "/Reviews/Create",
            type: "POST",
            data: $(this).serialize(),
            success: function (html) {
                $("#reviewsContainer").html(html);
                $("#reviewForm")[0].reset();

                // Reset stars
                $("#selectedRating").val(0);
                $(".star").removeClass("selected").css("color", "#ddd");
            }
        });
    });

    // Load edit modal
    $(document).on("click", ".edit-review-btn", function () {
        var id = $(this).data("id");

        $.get("/Reviews/Edit/" + id, function (modalHtml) {
            $("#modalContainer").html(modalHtml);
            $("#editReviewModal").modal("show");
        });
    });

    // Save edit
    $(document).on("submit", "#editReviewForm", function (e) {
        e.preventDefault();

        $.ajax({
            url: "/Reviews/Edit",
            type: "POST",
            data: $(this).serialize(),
            success: function (html) {
                $("#reviewsContainer").html(html);
                $("#editReviewModal").modal("hide");
            }
        });
    });

    // Delete review
    $(document).on("click", ".delete-review-btn", function () {
        if (!confirm("Are you sure?")) return;

        var id = $(this).data("id");

        $.ajax({
            url: "/Reviews/Delete/" + id,
            type: "POST",
            data: {
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (html) {
                $("#reviewsContainer").html(html);
            }
        });

    });

    // Click stars in Edit Modal
    $(document).on("click", "#editStarRating .star", function () {
        var value = $(this).data("value");

        $("#editSelectedRating").val(value);

        $("#editStarRating .star").each(function () {
            var starValue = $(this).data("value");
            $(this).css("color", starValue <= value ? "#E6BE8A" : "#ddd");
        });
    });

});
