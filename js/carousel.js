document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.carousel').forEach(initCarousel);
  });
  
  function initCarousel(carousel) {
    let currentSlide = 0;
    const slides = carousel.querySelectorAll('.carousel-slide');
    const track = carousel.querySelector('.carousel-track');
    const autoplay = carousel.dataset.autoplay !== "false";
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
  
  document.addEventListener('DOMContentLoaded', () => {
    const quoteBlock = document.querySelector('.carousel-quote');
    if (!quoteBlock) return;
  
    const quotes = quoteBlock.querySelectorAll('blockquote');
    let current = 0;
  
    function showQuote(index) {
      quotes.forEach((q, i) => {
        q.classList.toggle('active', i === index);
      });
    }
  
    function nextQuote() {
      current = (current + 1) % quotes.length;
      showQuote(current);
    }
  
    showQuote(current);
    setInterval(nextQuote, 8000); // change quote every 8s
  });

// Clickable image gallery

document.addEventListener('DOMContentLoaded', () => {
  const modal = document.createElement('div');
  modal.className = 'image-modal';

  const dots = document.createElement('div');
  dots.className = 'image-dots';
  modal.appendChild(dots);

  const img = document.createElement('img');
  const caption = document.createElement('div');
  caption.className = 'image-caption';

  const nav = document.createElement('div');
  nav.className = 'image-nav';

  const prevBtn = document.createElement('button');
  prevBtn.innerHTML = '‹';
  const nextBtn = document.createElement('button');
  nextBtn.innerHTML = '›';

  nav.appendChild(prevBtn);
  nav.appendChild(nextBtn);

  modal.appendChild(img);
  modal.appendChild(caption);
  modal.appendChild(nav);
  document.body.appendChild(modal);

  const images = Array.from(document.querySelectorAll('img.clickable'));
  let currentIndex = 0;

  function showImage(index) {
    const image = images[index];
    if (!image) return;

    currentIndex = index;
    img.src = image.src;
    caption.textContent = image.alt || '';
    modal.style.display = 'flex';
    dots.innerHTML = '';

    images.forEach((_, i) => {
      const dot = document.createElement('button');
      dot.className = 'image-dot';
      if (i === index) dot.classList.add('active');
      dot.addEventListener('click', (e) => {
        e.stopPropagation();
        showImage(i);
      });
      dots.appendChild(dot);
    });

  }

  function hideModal() {
    modal.style.display = 'none';
    img.src = '';
    caption.textContent = '';
  }

  images.forEach((image, index) => {
    image.addEventListener('click', () => showImage(index));
  });

  modal.addEventListener('click', (e) => {
    if (e.target === modal || e.target === caption) hideModal();
  });

  prevBtn.addEventListener('click', (e) => {
    e.stopPropagation();
    showImage((currentIndex - 1 + images.length) % images.length);
  });

  nextBtn.addEventListener('click', (e) => {
    e.stopPropagation();
    showImage((currentIndex + 1) % images.length);
  });

  document.addEventListener('keydown', (e) => {
    if (modal.style.display === 'flex') {
      if (e.key === 'ArrowLeft') prevBtn.click();
      if (e.key === 'ArrowRight') nextBtn.click();
      if (e.key === 'Escape') hideModal();
    }
  });
});
