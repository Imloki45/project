window.onload =function(){
  const bsignUp = document.getElementById('adminLogin');
  const signInButton = document.getElementById('empLogin');
  const container = document.getElementById('container');
  
  bsignUp.addEventListener('click' ,() => container.classList.add("right-panel-active"));
  signInButton.addEventListener('click', () => container.classList.remove("right-panel-active"));
  }