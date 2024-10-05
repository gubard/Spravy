export function play(array) {
    let audioContext = new (window.AudioContext || window.webkitAudioContext)();
    
    audioContext.decodeAudioData(array.buffer, function (buffer) {
        let source = audioContext.createBufferSource();
        source.buffer = buffer;
        source.connect(audioContext.destination);
        source.start(0);
    }, function (e) {
        console.error("Error with decoding audio data", e);
    });
}