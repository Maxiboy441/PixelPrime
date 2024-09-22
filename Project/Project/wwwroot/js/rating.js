document.addEventListener('DOMContentLoaded', () => {
    const modals = document.querySelectorAll('[id^=ratingModalCenter]'); // Select all modals

    modals.forEach(modal => {
        const movieId = modal.getAttribute('id').split('-')[1]; // Extract movieId from modal ID
        const starsContainer = document.getElementById(`ratingContainer-${movieId}`);
        const form = document.getElementById(`ratingForm-${movieId}`);
        const ratingValueInput = document.getElementById(`ratingValue-${movieId}`);

        // Create stars dynamically
        for (let i = 1; i <= 10; i++) {
            const star = document.createElement('span');
            star.classList.add('star');
            star.setAttribute('data-value', i);
            star.innerHTML = '&#9734;'; // Star symbol

            // Add event listeners for hovering and clicking
            star.addEventListener('mouseover', () => {
                highlightStars(i, starsContainer);
            });

            star.addEventListener('mouseout', () => {
                highlightStars(ratingValueInput.value, starsContainer);
            });

            star.addEventListener('click', () => {
                setRating(i, movieId);
            });

            starsContainer.appendChild(star);
        }

        function setRating(value, movieId) {
            ratingValueInput.value = value;
            highlightStars(value, starsContainer, 'selected');
            submitRatingForm(form, movieId, value);
        }

        function highlightStars(limit, container, className = 'hovered') {
            const stars = container.querySelectorAll('.star');
            stars.forEach((star, index) => {
                star.classList.toggle(className, index < limit);
            });
        }

        // Submit the form via AJAX (using fetch API)
        async function submitRatingForm(form, movieId, ratingValue) {
            const formData = new FormData(form);
            formData.append("ratingValue", ratingValue);

            try {
                const response = await fetch(form.action, {
                    method: 'POST',
                    body: formData,
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });

                const result = await response.json();

                if (result.success) {
                    // Update the displayed rating on the page
                    const ratingDisplay = document.querySelector(`#rating-value-${movieId}`);
                    if (ratingDisplay) {
                        ratingDisplay.textContent = `${parseFloat(result.ratingValue).toFixed(1).replace('.', ',')} / 10`;
                    } else {
                        const ratingBlock = document.querySelector('.rating-h6');
                        const newRatingDisplay = document.createElement('div'); // Create a div to hold the icon and text

                        // Create the star icon element
                        const starIcon = document.createElement('i');
                        starIcon.className = 'fa-solid fa-star';
                        starIcon.style.color = '#FFD43B';

                        // Create the span for the rating value
                        const ratingValueSpan = document.createElement('span');
                        ratingValueSpan.className = 'rate';
                        ratingValueSpan.id = `rating-value-${movieId}`;
                        ratingValueSpan.textContent = `${parseFloat(ratingValue).toFixed(1).replace('.', ',')} / 10`;

                        // Append the icon and span to the newRatingDisplay
                        newRatingDisplay.appendChild(starIcon);
                        newRatingDisplay.appendChild(ratingValueSpan);

                        // Append the newRatingDisplay to the rating block
                        ratingBlock.appendChild(newRatingDisplay);
                    }

                    // Display the flash message
                    flashMessage('alert-success', result.message);
                } else {
                    // Show error message
                    flashMessage('alert-danger', result.message);
                }
            } catch (error) {
                console.error('Error:', error);
                flashMessage('alert-danger', 'An error occurred while submitting your rating.');
            }
        }
    });

    function flashMessage(alertType, message) {
        const flashMessageContainer = document.createElement('div');
        flashMessageContainer.className = `alert ${alertType} alert-dismissible fade show custom-alert`;
        flashMessageContainer.role = 'alert';
        flashMessageContainer.innerHTML = `${message}<button type="button" class="btn btn-close" data-dismiss="alert" aria-label="Close" />`;

        const container = document.querySelector('.content-wrapper');
        container.appendChild(flashMessageContainer);

        setTimeout(() => {
            flashMessageContainer.classList.remove('show');
            flashMessageContainer.classList.add('fade');
            setTimeout(() => flashMessageContainer.remove(), 150);
        }, 3500);
    }
});
