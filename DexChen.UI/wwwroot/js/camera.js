// Module de gestion de la caméra pour DexChen
let videoElement;
let stream;
let facingMode = "environment"; // Par défaut, utiliser la caméra arrière

// Initialise la caméra et affiche le flux vidéo
export async function initCamera(videoElementId) {
    try {
        videoElement = document.getElementById(videoElementId);
        
        if (!videoElement) {
            throw new Error('Élément vidéo non trouvé');
        }
        
        // Vérification si la caméra est disponible
        if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
            throw new Error('L\'API MediaDevices n\'est pas prise en charge par ce navigateur');
        }
        
        // Arrêter le flux existant s'il y en a un
        if (stream) {
            stopCamera();
        }
        
        // Définir les contraintes de la caméra
        const constraints = {
            video: {
                facingMode: facingMode,
                width: { ideal: 1280 },
                height: { ideal: 720 }
            }
        };
        
        // Demander l'accès à la caméra
        stream = await navigator.mediaDevices.getUserMedia(constraints);
        videoElement.srcObject = stream;
        
        // Attendre que la vidéo soit chargée
        return new Promise((resolve) => {
            videoElement.onloadedmetadata = () => {
                videoElement.play();
                resolve();
            };
        });
    } catch (error) {
        console.error('Erreur lors de l\'initialisation de la caméra:', error);
        throw error;
    }
}

// Arrête la caméra et libère les ressources
export function stopCamera() {
    if (stream) {
        stream.getTracks().forEach(track => {
            track.stop();
        });
        stream = null;
    }
    
    if (videoElement) {
        videoElement.srcObject = null;
    }
}

// Bascule entre la caméra avant et arrière
export async function switchCamera() {
    // Changer le mode de la caméra
    facingMode = facingMode === "environment" ? "user" : "environment";
    
    // Réinitialiser la caméra
    if (videoElement) {
        await initCamera(videoElement.id);
    }
}

// Capture une image depuis la vidéo
export function captureImage() {
    if (!videoElement) {
        throw new Error('La vidéo n\'est pas initialisée');
    }
    
    try {
        // Créer un canvas pour capturer l'image
        const canvas = document.createElement('canvas');
        canvas.width = videoElement.videoWidth;
        canvas.height = videoElement.videoHeight;
        
        // Dessiner l'image vidéo sur le canvas
        const context = canvas.getContext('2d');
        context.drawImage(videoElement, 0, 0, canvas.width, canvas.height);
        
        // Convertir en base64
        return canvas.toDataURL('image/jpeg', 0.9);
    } catch (error) {
        console.error('Erreur lors de la capture d\'image:', error);
        throw error;
    }
} 