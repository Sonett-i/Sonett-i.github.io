document.addEventListener("DOMContentLoaded", () => {
    const deckCards = document.querySelectorAll(".deck-card[id]");
    if (!deckCards.length) return;
  
    const nav = document.createElement("nav");
    nav.className = "sidebar-nav";
  
    const list = document.createElement("ul");
  
    deckCards.forEach(card => {
      const id = card.id;
      const title = card.querySelector("h3")?.textContent || id;
  
      const li = document.createElement("li");
      const link = document.createElement("a");
      link.href = `#${id}`;
      link.textContent = title;
  
      li.appendChild(link);
      list.appendChild(li);
    });
  
    nav.appendChild(list);
  
    // Optional smooth scrolling
    nav.addEventListener("click", (e) => {
      if (e.target.tagName === "A") {
        e.preventDefault();
        const target = document.querySelector(e.target.getAttribute("href"));
        target?.scrollIntoView({ behavior: "smooth" });
      }
    });
  
    // Insert before your <main>
    const wrapper = document.createElement("div");
    wrapper.className = "content-wrapper";
    const main = document.querySelector("main");
    main.parentNode.insertBefore(wrapper, main);
    wrapper.appendChild(nav);
    wrapper.appendChild(main);
  });
  