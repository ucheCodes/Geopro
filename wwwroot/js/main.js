    // Global data container
    window.globalDataContainer = [];

    // Function to add data to the container
    function addData(data) {
        window.globalDataContainer.push(data);
    }

    // Function to get all data from the container
    function getData() {
        return window.globalDataContainer;
    }

    // Function to clear the container
    function clearData() {
        window.globalDataContainer = [];
    }
    //get length of data
    function removeLastData() {
        return window.globalDataContainer.pop();
    }

window.saveAsFile = function (fileName, fileContent) {
    const blob = new Blob([fileContent], { type: 'text/plain' });
    const anchor = document.createElement('a');
    anchor.href = window.URL.createObjectURL(blob);
    anchor.download = fileName;
    anchor.click();
    window.URL.revokeObjectURL(anchor.href);
};

window.blazorExtensions = {
    captureAndConvertToPDF: async function (filename) {

        const { jsPDF } = window.jspdf;
        const pdf = new jsPDF('p', 'mm', 'a4');

       let  pageNumber = 0;
        window.globalDataContainer.forEach(imgData => {
            // Add the captured image to the PDF
            const imgProps = pdf.getImageProperties(imgData);
            const pdfWidth = pdf.internal.pageSize.getWidth();
            const pdfHeight = (imgProps.height * pdfWidth) / imgProps.width;
            pdf.addImage(imgData, 'PNG', 0, 0, pdfWidth, pdfHeight);
            pageNumber++;
            pdf.setFontSize(10);
            pdf.text(`Page ${pageNumber}`, pdfWidth - 20, pdf.internal.pageSize.getHeight() - 10);
            pdf.addPage();
        }); 
        clearData(); 
            // Save the PDF
        pdf.save(filename+'.pdf');


        /*console.log("hello adding");
        const element = document.getElementById(id);
        if (element) {
            console.log(element.outerHTML);
        }*/
    },
    addHtmlContent: async function (id)
    {
        const canvas = await html2canvas(document.querySelector("#"+id));
        const imgData = canvas.toDataURL('image/png');
        addData(imgData);
        /*var del = window.globalDataContainer.pop();
        console.log("removed: "+del);*/
        return window.globalDataContainer.length;
    }
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

//change select
function setSelectValue(element, value) {
    element.value = value;
    var event = new Event('change');
    element.dispatchEvent(event);
}