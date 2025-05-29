document.addEventListener("DOMContentLoaded", () => {
    const token = localStorage.getItem("jwtToken");
    if (!token) return;

    fetch("https://localhost:7273/api/Profile/Get_Picture", {
        headers: {
            Authorization: `Bearer ${token}`
        }
    })
        .then(res => res.json())
        .then(data => {
            const img = document.getElementById("profilePic");
            if (img && data.pictureUrl) {
                img.src = `https://localhost:7273${data.pictureUrl}`;
                console.log("✅ الصورة ظهرت بنجاح");
            }
        })
        .catch(err => {
            console.error("❌ فشل تحميل الصورة:", err.message);
        });
});
