const width = window.innerWidth - 250;
const height = window.innerHeight;

const stage = new Konva.Stage({ container: 'container', width, height });
const videoLayer = new Konva.Layer();
const annotationLayer = new Konva.Layer();
stage.add(videoLayer);
stage.add(annotationLayer);

// Flux vidéo
/*
const videoElement = document.createElement('video');
videoElement.autoplay = true;
videoElement.playsInline = true;
navigator.mediaDevices.getUserMedia({ video: true }).then(stream => videoElement.srcObject = stream);

const videoNode = new Konva.Image({ image: videoElement, width, height });
videoLayer.add(videoNode);
const anim = new Konva.Animation(() => {}, videoLayer);
anim.start();
//*/

// Flux video II
const videoElement = document.createElement('img');
videoElement.src = 'https://localhost:7083/mjpeg-stream'; // Remplace par ton URL MJPEG
videoElement.crossOrigin = 'anonymous';        // si nécessaire pour éviter les problèmes CORS

const videoNode = new Konva.Image({
  image: videoElement,
  width,
  height
});
videoLayer.add(videoNode);


videoElement.onload = function(e){
  videoLayer.batchDraw();
};

// Gestion des outils
let circle3PointsClicks = [];
let currentTool = 'rect';

document.getElementById('drawRect').onclick = () => currentTool = 'rect';
document.getElementById('drawCircle').onclick = () => currentTool = 'circle';
document.getElementById('drawText').onclick = () => currentTool = 'text';
document.getElementById('drawLine').onclick = () => currentTool = 'line';
document.getElementById('drawCircle3Points').onclick = () => {
  currentTool = 'circle3Points';
  circle3PointsClicks = [];
};

let startX, startY, currentShape;
const shapeList = document.getElementById('shapeList');

function addShapeToList(shape) {
  const li = document.createElement('li');
  li.textContent = shape.getClassName() + ' #' + shape._id;
  li.onclick = () => {
    stage.find('.selected').forEach(s => s.className = '');
    shape.draggable(true);
    li.classList.add('selected');
  };
  shapeList.appendChild(li);
  shape._listItem = li;
}

let shapeIdCounter = 1;

stage.on('mousedown', e => {
  startX = getTransformedPointerPosition().x;
  startY = getTransformedPointerPosition().y;

  console.log(e);

  if(e.target.attrs.image == null)
    return;

  console.log(e);

  if (currentTool === 'rect') {
    currentShape = new Konva.Rect({
      x: startX,
      y: startY,
      width: 0,
      height: 0,
      stroke: 'red',
      strokeWidth: 2,
      draggable: true,
      _id: shapeIdCounter++
    });
  } else if (currentTool === 'circle') {
    currentShape = new Konva.Circle({
      x: startX,
      y: startY,
      radius: 0,
      stroke: 'green',
      strokeWidth: 2,
      draggable: true,
      _id: shapeIdCounter++
    });
  } else if (currentTool === 'text') {
    const text = prompt('Texte à ajouter ?');
    if (!text) return;
    currentShape = new Konva.Text({
      x: startX,
      y: startY,
      text: text,
      fontSize: 18,
      fill: 'blue',
      draggable: true,
      _id: shapeIdCounter++
    });
    addShapeToList(currentShape);
    annotationLayer.add(currentShape);
    annotationLayer.draw();
    return;
  } else if (currentTool === 'line') {
    currentShape = new Konva.Line({
      points: [startX, startY, startX, startY],
      stroke: 'orange',
      strokeWidth: 2,
      draggable: true,
      _id: shapeIdCounter++
    });
    annotationLayer.add(currentShape);
  } else if(currentTool == 'circle3Points'){
    const pos = getTransformedPointerPosition();
    circle3PointsClicks.push(pos);

    if (circle3PointsClicks.length === 3) {
      const circleData = circleFrom3Points(
        circle3PointsClicks[0],
        circle3PointsClicks[1],
        circle3PointsClicks[2]
      );

      if (!circleData) {
        alert('Les points sont alignés, impossible de créer un cercle.');
        circle3PointsClicks = [];
        return;
      }

      const circle = new Konva.Circle({
        x: circleData.x,
        y: circleData.y,
        radius: circleData.radius,
        stroke: 'purple',
        strokeWidth: 2,
        draggable: true,
        _id: shapeIdCounter++
      });

      annotationLayer.add(circle);
      addShapeToList(circle);
      annotationLayer.draw();
      circle3PointsClicks = [];
    }
  }

  if(currentShape != null)
    annotationLayer.add(currentShape);
});

stage.on('mousemove', e => {
  if (!currentShape) return;
  const pos = getTransformedPointerPosition();
  if (currentTool === 'rect') {
    currentShape.width(pos.x - startX);
    currentShape.height(pos.y - startY);
  } else if (currentTool === 'circle') {
    currentShape.radius(Math.sqrt(Math.pow(pos.x - startX, 2) + Math.pow(pos.y - startY, 2)));
    currentShape.x(startX);
    currentShape.y(startY);
  } else if (currentTool === 'line') {
    if (!currentShape) return;
    if (currentTool === 'line') {
      currentShape.points([startX, startY, pos.x, pos.y]);
    }
  }
  annotationLayer.batchDraw();
});

stage.on('mouseup', () => {
  if(currentTool == 'circle3Points') return;
  if (!currentShape) return;
  addShapeToList(currentShape);
  console.log(currentShape);
  currentShape = null;
  // circle:radius rectangle:width height line:points
});

stage.on('wheel', e => {
  e.evt.preventDefault();
  const oldScale = stage.scaleX();
  const pointer = stage.getPointerPosition();

  const scaleBy = 1.05;
  const direction = e.evt.deltaY > 0 ? 1 / scaleBy : scaleBy;
  const newScale = oldScale * direction;

  stage.scale({ x: newScale, y: newScale });

  const mousePointTo = {
    x: (pointer.x - stage.x()) / oldScale,
    y: (pointer.y - stage.y()) / oldScale,
  };

  stage.position({
    x: pointer.x - mousePointTo.x * newScale,
    y: pointer.y - mousePointTo.y * newScale,
  });

  stage.batchDraw();
});

const transformer = new Konva.Transformer();
annotationLayer.add(transformer);

stage.on('click', e => {
  // Ne sélectionner que les formes d'annotation
  if (e.target === stage || e.target === videoNode) {
    transformer.nodes([]);
    annotationLayer.draw();
    return;
  }

  transformer.nodes([e.target]);
  annotationLayer.draw();
});


window.addEventListener('keydown', e => {
  if (e.key === 'Delete' || e.key === 'Backspace') {
    const selectedNodes = transformer.nodes();
    selectedNodes.forEach(node => {
      node._listItem.remove(); // retire de la liste
      node.destroy();          // retire du layer
    });
    transformer.nodes([]);
    annotationLayer.draw();
  }
  if (e.key === 'Escape' && currentTool === 'circle3Points') {
    circle3PointsClicks = [];
  }
});

function circleFrom3Points(p1, p2, p3) {
  const A = p2.x - p1.x;
  const B = p2.y - p1.y;
  const C = p3.x - p1.x;
  const D = p3.y - p1.y;
  const E = A*(p1.x + p2.x) + B*(p1.y + p2.y);
  const F = C*(p1.x + p3.x) + D*(p1.y + p3.y);
  const G = 2*(A*(p3.y - p2.y) - B*(p3.x - p2.x));
  if (G === 0) return null; // points alignés
  const cx = (D*E - B*F) / G;
  const cy = (A*F - C*E) / G;
  const radius = Math.sqrt(Math.pow(cx - p1.x, 2) + Math.pow(cy - p1.y, 2));
  return { x: cx, y: cy, radius };
}

function getTransformedPointerPosition() {
  const pos = stage.getPointerPosition();
  const scale = stage.scaleX(); // on suppose scaleX = scaleY
  return {
    x: (pos.x - stage.x()) / scale,
    y: (pos.y - stage.y()) / scale
  };
}

