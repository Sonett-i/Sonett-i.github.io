document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll('code[data-src]').forEach(async (codeBlock) => {
      const src = codeBlock.getAttribute('data-src');
      const wrapper = codeBlock.parentElement;
  
      try {
        const response = await fetch(src);
        if (!response.ok) throw new Error(`Failed to load ${src}`);
        const code = await response.text();
        codeBlock.textContent = code;
        Prism.highlightElement(codeBlock);
  
        if (wrapper.classList.contains('line-numbers')) {
          Prism.plugins.lineNumbers.resize(wrapper);
        }
  
        // Add copy button
        const copyBtn = document.createElement('button');
        copyBtn.textContent = 'Copy';
        copyBtn.className = 'copy-btn';
        copyBtn.addEventListener('click', () => {
          navigator.clipboard.writeText(code).then(() => {
            copyBtn.textContent = 'Copied!';
            setTimeout(() => (copyBtn.textContent = 'Copy'), 1500);
          });
        });
        wrapper.appendChild(copyBtn);
  
      } catch (err) {
        codeBlock.textContent = `/* Error loading ${src}: ${err.message} */`;
      }
    });
  });
  