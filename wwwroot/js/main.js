// Add this JavaScript code in your Blazor application
window.saveAsFile = function (fileName, fileContent) {
    const blob = new Blob([fileContent], { type: 'text/plain' });
    const anchor = document.createElement('a');
    anchor.href = window.URL.createObjectURL(blob);
    anchor.download = fileName;
    anchor.click();
    window.URL.revokeObjectURL(anchor.href);
};


function showFileDialog() {
    document.getElementById('fileInput').click();
}
function openFileDialog() {
    document.getElementById('input').click();
}
function openImageDialog() {
    document.getElementById('image').click();
}
function screenShot(chartId){
    let chart = document.getElementById(chartId);//"chart1"
    html2canvas(chart).then(
        function (canvas) {
            document.getElementById('output').appendChild(canvas);
        }
    )
}