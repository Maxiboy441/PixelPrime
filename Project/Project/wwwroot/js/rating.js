document.addEventListener('DOMContentLoaded', () => {
    const starsContainer = document.getElementById('ratingContainer');
    const form = document.getElementById('ratingForm');

    // Create stars dynamically
    for (let i = 1; i <= 10; i++) {
        const star = document.createElement('span');
        star.classList.add('star');
        star.setAttribute('data-value', i);
        star.innerHTML = '&#9734;';

        star.addEventListener('mouseover', () => {
            highlightStars(i);
        });

        star.addEventListener('mouseout', () => {
            highlightStars(document.getElementById('ratingValue').value);
        });

        star.addEventListener('click', () => {
            setRating(i);
        });

        starsContainer.appendChild(star);
    }

    function setRating(value) {
        document.getElementById('ratingValue').value = value;
        highlightStars(value, 'selected');
        form.submit();
    }

    function highlightStars(limit, className = 'hovered') {
        const stars = starsContainer.querySelectorAll('.star');
        stars.forEach((star, index) => {
            star.classList.toggle(className, index < limit);
        });
    }
});