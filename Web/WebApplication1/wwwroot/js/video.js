const fileUploader = document.getElementById('video_file');

fileUploader.addEventListener('change', (event) => {
  const files = event.target.files;
  console.log('files', files);
  
  // show the upload feedback
  const feedback = document.getElementById('feedback2');
  const msg = `${files.length} Файл(ы) успешно загруженны!`;
            feedback.innerHTML = msg;
});