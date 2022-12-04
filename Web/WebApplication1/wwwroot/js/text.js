const fileUploader = document.getElementById('text_files');

fileUploader.addEventListener('change', (event) => {
  const files = event.target.files;
  console.log('files', files);
  
  // show the upload feedback
  const feedback = document.getElementById('feedback');
  const msg = `${files.length}Файл(ы) успешно загруженны!`;
            feedback.innerHTML = msg;
});