document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.carousel').forEach(initCarousel);
  });
  
  function initCarousel(carousel) {
    let currentSlide = 0;
    const slides = carousel.querySelectorAll('.carousel-slide');
    const track = carousel.querySelector('.carousel-track');
    const autoplay = carousel.dataset.autoplay !== undefined;
    const useFade = carousel.classList.contains('fade');
    let intervalId = null;
  
    const nextBtn = carousel.querySelector('.carousel-btn.next');
    const prevBtn = carousel.querySelector('.carousel-btn.prev');
    const dotsContainer = carousel.querySelector('.carousel-dots');
  
    function updateCarousel() {
      if (useFade) {
        slides.forEach((s, i) => {
          s.style.opacity = i === currentSlide ? '1' : '0';
          s.style.zIndex = i === currentSlide ? '1' : '0';
        });
      } else {
        track.style.transform = `translateX(-${currentSlide * 100}%)`;
      }
  
      if (dotsContainer) {
        dotsContainer.querySelectorAll('button').forEach((dot, i) => {
          dot.classList.toggle('active', i === currentSlide);
        });
      }
    }
  
    function next() {
      currentSlide = (currentSlide + 1) % slides.length;
      updateCarousel();
    }
  
    function prev() {
      currentSlide = (currentSlide === 0) ? slides.length - 1 : currentSlide - 1;
      updateCarousel();
    }
  
    function goToSlide(index) {
      currentSlide = index;
      updateCarousel();
      resetAutoplay();
    }
  
    function startAutoplay() {
      if (autoplay && slides.length > 1) {
        intervalId = setInterval(next, 5000);
      }
    }
  
    function stopAutoplay() {
      if (intervalId) clearInterval(intervalId);
      intervalId = null;
    }
  
    function resetAutoplay() {
      stopAutoplay();
      startAutoplay();
    }
  
    if (nextBtn) nextBtn.addEventListener('click', () => {
      next();
      resetAutoplay();
    });
  
    if (prevBtn) prevBtn.addEventListener('click', () => {
      prev();
      resetAutoplay();
    });
  
    // Dot Nav
    if (dotsContainer) {
      slides.forEach((_, i) => {
        const dot = document.createElement('button');
        dot.addEventListener('click', () => goToSlide(i));
        dotsContainer.appendChild(dot);
      });
    }
  
    // Pause on hover
    carousel.addEventListener('mouseenter', stopAutoplay);
    carousel.addEventListener('mouseleave', startAutoplay);
  
    // Touch swipe support
    let startX = 0;
    carousel.addEventListener('touchstart', e => {
      startX = e.touches[0].clientX;
    });
  
    carousel.addEventListener('touchend', e => {
      const endX = e.changedTouches[0].clientX;
      const deltaX = endX - startX;
  
      if (Math.abs(deltaX) > 50) {
        deltaX < 0 ? next() : prev();
        resetAutoplay();
      }
    });
  
    updateCarousel();
    startAutoplay();
  }
  