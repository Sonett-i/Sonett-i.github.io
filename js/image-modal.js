document.addEventListener("DOMContentLoaded", () => {
    const lightbox = document.getElementById("lightbox");
    const img = lightbox.querySelector(".lightbox-image");
    const video = lightbox.querySelector(".lightbox-video");
  
    document.querySelectorAll(".clickable").forEach(el => {
      el.addEventListener("click", () => {
        const isVideo = el.tagName.toLowerCase() === "video";
  
        lightbox.style.display = "flex";
        if (isVideo) {
          img.style.display = "none";
          video.style.display = "block";
          video.src = el.querySelector("source")?.src || el.src;
          video.load();
          video.play();
        } else {
          video.pause();
          video.style.display = "none";
          img.style.display = "block";
          img.src = el.src;
          img.alt = el.alt;
        }
      });
    });
  
    lightbox.addEventListener("click", closeLightbox);
  });
  
  function closeLightbox() {
    const lightbox = document.getElementById("lightbox");
    const video = lightbox.querySelector(".lightbox-video");
    video.pause();
    lightbox.style.display = "none";
  }
  
  document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('video.clickable').forEach(video => {
      const wrapper = document.createElement('div');
      wrapper.className = 'video-overlay-wrapper';
  
      const icon = document.createElement('div');
      icon.className = 'video-play-icon';
      icon.innerHTML = '▶'; // Or use a play symbol SVG if preferred
  
      video.parentNode.insertBefore(wrapper, video);
      wrapper.appendChild(video);
      wrapper.appendChild(icon);
    });
  });
  