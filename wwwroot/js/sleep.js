function sleep(ms) {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            resolve(ms);
        }, ms);
    })
}
async function myFunction() {
    await sleep(5000);
}
myFunction();