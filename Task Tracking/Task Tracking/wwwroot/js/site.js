

    window.addEventListener('scroll', function () {
        const nav = document.getElementById('mainNav');
        if (window.scrollY > 50) {
        nav.classList.add('navbar-scrolled');
        } else {
        nav.classList.remove('navbar-scrolled');
        }
    });



window.addEventListener('DOMContentLoaded', event => {

    // Navbar shrink function
    var navbarShrink = function () {
        const navbarCollapsible = document.body.querySelector('#mainNav');
        if (!navbarCollapsible) {
            return;
        }
        if (window.scrollY === 0) {
            navbarCollapsible.classList.remove('navbar-shrink')
        } else {
            navbarCollapsible.classList.add('navbar-shrink')
        }

    };

    // Shrink the navbar 
    navbarShrink();

    // Shrink the navbar when page is scrolled
    document.addEventListener('scroll', navbarShrink);

    // Activate Bootstrap scrollspy on the main nav element
    const mainNav = document.body.querySelector('#mainNav');
    if (mainNav) {
        new bootstrap.ScrollSpy(document.body, {
            target: '#mainNav',
            rootMargin: '0px 0px -40%',
        });
    };

    // Collapse responsive navbar when toggler is visible
    const navbarToggler = document.body.querySelector('.navbar-toggler');
    const responsiveNavItems = [].slice.call(
        document.querySelectorAll('#navbarResponsive .nav-link')
    );
    responsiveNavItems.map(function (responsiveNavItem) {
        responsiveNavItem.addEventListener('click', () => {
            if (window.getComputedStyle(navbarToggler).display !== 'none') {
                navbarToggler.click();
            }
        });
    });


    // Home
    document.addEventListener('DOMContentLoaded', function () {
        const track = document.querySelector('.testimonials-track');
        const cards = document.querySelectorAll('.testimonial-card');
        const prevBtn = document.querySelector('.prev-btn');
        const nextBtn = document.querySelector('.next-btn');
        const indicators = document.querySelectorAll('.indicator');

        let currentIndex = 0;
        const cardCount = cards.length;
        let cardsPerView = 3;

        // Function to update cards per view based on screen size
        function updateCardsPerView() {
            if (window.innerWidth < 768) {
                cardsPerView = 1;
            } else if (window.innerWidth < 992) {
                cardsPerView = 2;
            } else {
                cardsPerView = 3;
            }
            updateSlider();
        }

        // Initialize on load
        updateCardsPerView();

        // Update on resize
        window.addEventListener('resize', updateCardsPerView);

        // Function to update slider position
        function updateSlider() {
            const cardWidth = cards[0].offsetWidth + 30; // width + margin
            track.style.transform = `translateX(-${currentIndex * cardWidth}px)`;

            // Update indicators
            indicators.forEach((indicator, index) => {
                indicator.classList.toggle('active', index === currentIndex);
            });
        }

        // Next button event
        nextBtn.addEventListener('click', function () {
            if (currentIndex < Math.ceil(cardCount / cardsPerView) - 1) {
                currentIndex++;
                updateSlider();
            }
        });

        // Previous button event
        prevBtn.addEventListener('click', function () {
            if (currentIndex > 0) {
                currentIndex--;
                updateSlider();
            }
        });

        // Indicator click events
        indicators.forEach((indicator, index) => {
            indicator.addEventListener('click', function () {
                currentIndex = index;
                updateSlider();
            });
        });

        // Auto-advance testimonials
        setInterval(function () {
            if (currentIndex < Math.ceil(cardCount / cardsPerView) - 1) {
                currentIndex++;
            } else {
                currentIndex = 0;
            }
            updateSlider();
        }, 5000);
    });
});