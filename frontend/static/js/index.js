document.getElementById('catBtn').addEventListener('click', function() {
    document.getElementById('someImg').src = '../static/imgs/cat.jpg';
    document.getElementById('someImg').classList.remove("hidden")
});

document.getElementById('dogBtn').addEventListener('click', function() {
    document.getElementById('someImg').src = '../static/imgs/dog.jpg';
    document.getElementById('someImg').classList.remove("hidden")
});
