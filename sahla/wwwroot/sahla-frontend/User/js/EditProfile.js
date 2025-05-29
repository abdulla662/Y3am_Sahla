document.addEventListener('DOMContentLoaded', () => {
    const API_BASE_URL = 'https://localhost:7273/api/Profile';
    const token = localStorage.getItem('jwtToken');

    // لو مفيش توكن رجّع المستخدم للّوجين
    if (!token) {
        window.location.href = '/sahla-frontend/User/html/login.html';
        return;
    }

    // استدعاء بيانات البروفايل
    fetch(`${API_BASE_URL}/Profile`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
    })
    .then(res => {
        if (!res.ok) throw new Error('Unauthorized');
        return res.json();
    })
    .then(data => {
        document.getElementById('email').value = data.email || '';
        document.getElementById('address').value = data.adress || '';
        document.getElementById('points').value = data.points ?? 'N/A';
        document.getElementById('created-at').value = data.createdAt
            ? new Date(data.createdAt).toLocaleDateString()
            : 'N/A';

        // لو عندك عنصر صورة حقيقية لعرض الصورة
        const img = document.querySelector('.profile-pic');
        if (img && data.profilePicture) {
            img.src = `https://localhost:7273${data.profilePicture}`;
        }
    })
    .catch(error => {
        console.error('Error fetching profile:', error);
        alert('Unauthorized access. Redirecting to login...');
        window.location.href = '/sahla-frontend/User/html/login.html';
    });

    // حفظ التعديلات
    document.querySelector('.save-button').addEventListener('click', async () => {
        const fileInput = document.getElementById('profile-picture');
        const addressInput = document.getElementById('address');
        const formData = new FormData();

        if (fileInput.files && fileInput.files.length > 0) {
            formData.append('profilePicture', fileInput.files[0]);
        }

        formData.append('address', addressInput.value.trim());

        try {
            const response = await fetch(`${API_BASE_URL}/Update_Picture`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`
                    // لا تضيف Content-Type مع FormData!
                },
                body: formData
            });

            if (response.ok) {
                alert('Profile updated successfully!');
                window.location.reload();
            } else {
                const err = await response.text();
                alert('Update failed: ' + err);
            }
        } catch (error) {
            console.error('Upload error:', error);
            alert('Something went wrong while updating your profile.');
        }
    });
});
