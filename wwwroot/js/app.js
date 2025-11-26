let students = [];
let canteens = [];
let reservations = [];
document.addEventListener('DOMContentLoaded', () => {
    initializeNavigation();
    initializeFormHandlers();
    loadInitialData();
});
function initializeNavigation() {
    const navLinks = document.querySelectorAll('.nav-link');
    navLinks.forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            navLinks.forEach(l => l.classList.remove('active'));
            link.classList.add('active');
            const pageName = link.getAttribute('data-page');
            document.querySelectorAll('.page').forEach(page => {
                page.classList.remove('active');
            });
            document.getElementById(`${pageName}-page`).classList.add('active');
        });
    });
}

function initializeFormHandlers() {
    document.getElementById('studentForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const studentData = {
            name: document.getElementById('studentName').value,
            email: document.getElementById('studentEmail').value,
            isAdmin: document.getElementById('studentIsAdmin').checked
        };
        
        try {
            await StudentsAPI.create(studentData);
            showAlert('Student added successfully!', 'success');
            e.target.reset();
            loadStudents();
        } catch (error) {
            showAlert('Failed to add student', 'error');
        }
    });
    document.getElementById('canteenForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        const workingHoursItems = document.querySelectorAll('.working-hours-item');
        const workingHours = Array.from(workingHoursItems).map(item => ({
            meal: item.querySelector('.meal-type').value,
            from: item.querySelector('.from-time').value,
            to: item.querySelector('.to-time').value
        })).filter(wh => wh.meal && wh.from && wh.to);

        if (workingHours.length === 0) {
            showAlert('Please add at least one working hour', 'warning');
            return;
        }
        
        const canteenData = {
            name: document.getElementById('canteenName').value,
            location: document.getElementById('canteenLocation').value,
            capacity: parseInt(document.getElementById('canteenCapacity').value),
            workingHours: workingHours
        };
        
        try {
            await CanteensAPI.create(canteenData);
            showAlert('Canteen added successfully!', 'success');
            e.target.reset();
            resetWorkingHours();
            loadCanteens();
        } catch (error) {
            showAlert('Failed to add canteen', 'error');
        }
    });
    document.getElementById('addWorkingHours').addEventListener('click', (e) => {
        e.preventDefault();
        addWorkingHourField();
    });
    document.getElementById('reservationForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const reservationData = {
            studentId: document.getElementById('reservationStudentId').value,
            canteenId: document.getElementById('reservationCanteenId').value,
            date: document.getElementById('reservationDate').value,
            time: document.getElementById('reservationTime').value,
            duration: parseInt(document.getElementById('reservationDuration').value)
        };
        
        try {
            await ReservationsAPI.create(reservationData);
            showAlert('Reservation created successfully!', 'success');
            e.target.reset();
            loadReservations();
        } catch (error) {
            showAlert('Failed to create reservation', 'error');
        }
    });
}

async function loadInitialData() {
    await loadStudents();
    await loadCanteens();
    await loadReservations();
}

async function loadStudents() {
    try {
        students = await StudentsAPI.getAll();
        renderStudentsTable(students);
        populateStudentDropdown(students);
    } catch (error) {
        console.error('Error loading students:', error);
    }
}

async function loadCanteens() {
    try {
        canteens = await CanteensAPI.getAll();
        renderCanteensTable(canteens);
        populateCanteenDropdown(canteens);
    } catch (error) {
        console.error('Error loading canteens:', error);
    }
}

async function loadReservations() {
    try {
        reservations = await ReservationsAPI.getAll();
        renderReservationsTable(reservations);
    } catch (error) {
        console.error('Error loading reservations:', error);
    }
}

function renderStudentsTable(studentsList) {
    const tbody = document.querySelector('#studentsList tbody');
    
    if (studentsList.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" class="no-data">No students found</td></tr>';
        return;
    }
    
    tbody.innerHTML = studentsList.map(student => `
        <tr>
            <td>${student.id}</td>
            <td>${student.name}</td>
            <td>${student.email}</td>
            <td>${student.isAdmin ? 'Yes' : 'No'}</td>
            <td class="table-actions">
                <button class="btn btn-warning btn-sm" onclick="editStudent('${student.id}')">Edit</button>
                <button class="btn btn-danger btn-sm" onclick="deleteStudent('${student.id}')">Delete</button>
            </td>
        </tr>
    `).join('');
}

function renderCanteensTable(canteensList) {
    const tbody = document.querySelector('#canteensList tbody');
    
    if (canteensList.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6" class="no-data">No canteens found</td></tr>';
        return;
    }
    
    tbody.innerHTML = canteensList.map(canteen => {
        let hoursDisplay = 'N/A';
        if (canteen.workingHours && canteen.workingHours.length > 0) {
            hoursDisplay = canteen.workingHours.map(wh => 
                `${wh.meal}: ${wh.from}-${wh.to}`
            ).join(', ');
        }
        
        return `
        <tr>
            <td>${canteen.id}</td>
            <td>${canteen.name}</td>
            <td>${canteen.location}</td>
            <td>${canteen.capacity}</td>
            <td title="${hoursDisplay}">${hoursDisplay}</td>
            <td class="table-actions">
                <button class="btn btn-warning btn-sm" onclick="editCanteen('${canteen.id}')">Edit</button>
                <button class="btn btn-danger btn-sm" onclick="deleteCanteen('${canteen.id}')">Delete</button>
            </td>
        </tr>
    `}).join('');
}

function renderReservationsTable(reservationsList) {
    const tbody = document.querySelector('#reservationsList tbody');
    
    if (reservationsList.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8" class="no-data">No reservations found</td></tr>';
        return;
    }
    
    tbody.innerHTML = reservationsList.map(reservation => `
        <tr>
            <td>${reservation.id}</td>
            <td>${reservation.studentName || reservation.studentId}</td>
            <td>${reservation.canteenName || reservation.canteenId}</td>
            <td>${reservation.date}</td>
            <td>${reservation.time}</td>
            <td>${reservation.duration} min</td>
            <td>
                <span class="status-badge status-${reservation.status.toLowerCase()}">
                    ${reservation.status}
                </span>
            </td>
            <td class="table-actions">
                <button class="btn btn-danger btn-sm" onclick="cancelReservation('${reservation.id}')">Cancel</button>
            </td>
        </tr>
    `).join('');
}

function populateStudentDropdown(studentsList) {
    const select = document.getElementById('reservationStudentId');
    select.innerHTML = '<option value="">Select Student</option>' +
        studentsList.map(student => `<option value="${student.id}">${student.name}</option>`).join('');
}

function populateCanteenDropdown(canteensList) {
    const select = document.getElementById('reservationCanteenId');
    select.innerHTML = '<option value="">Select Canteen</option>' +
        canteensList.map(canteen => `<option value="${canteen.id}">${canteen.name}</option>`).join('');
}

async function deleteStudent(id) {
    if (!confirm('Are you sure you want to delete this student?')) return;
    
    try {
        await StudentsAPI.delete(id);
        showAlert('Student deleted successfully!', 'success');
        loadStudents();
    } catch (error) {
        showAlert('Failed to delete student', 'error');
    }
}

async function deleteCanteen(id) {
    if (!confirm('Are you sure you want to delete this canteen?')) return;
    
    try {
        await CanteensAPI.delete(id);
        showAlert('Canteen deleted successfully!', 'success');
        loadCanteens();
    } catch (error) {
        showAlert('Failed to delete canteen', 'error');
    }
}

async function cancelReservation(id) {
    if (!confirm('Are you sure you want to cancel this reservation?')) return;
    
    try {
        await ReservationsAPI.cancel(id);
        showAlert('Reservation cancelled successfully!', 'success');
        loadReservations();
    } catch (error) {
        showAlert('Failed to cancel reservation', 'error');
    }
}

function editStudent(id) {
    showAlert('Edit feature coming soon!', 'warning');
}

function editCanteen(id) {
    showAlert('Edit feature coming soon!', 'warning');
}

function addWorkingHourField() {
    const container = document.getElementById('workingHoursContainer');
    const newItem = document.createElement('div');
    newItem.className = 'working-hours-item';
    newItem.innerHTML = `
        <select class="meal-type" required>
            <option value="">Select Meal</option>
            <option value="breakfast">Breakfast</option>
            <option value="lunch">Lunch</option>
            <option value="dinner">Dinner</option>
        </select>
        <input type="time" class="from-time" placeholder="From" required>
        <input type="time" class="to-time" placeholder="To" required>
        <button type="button" class="btn btn-small btn-danger remove-time">Remove</button>
    `;
    
    newItem.querySelector('.remove-time').addEventListener('click', (e) => {
        e.preventDefault();
        newItem.remove();
    });
    
    container.appendChild(newItem);
}

function resetWorkingHours() {
    const container = document.getElementById('workingHoursContainer');
    container.innerHTML = `
        <div class="working-hours-item">
            <select class="meal-type" required>
                <option value="">Select Meal</option>
                <option value="breakfast">Breakfast</option>
                <option value="lunch">Lunch</option>
                <option value="dinner">Dinner</option>
            </select>
            <input type="time" class="from-time" placeholder="From" required>
            <input type="time" class="to-time" placeholder="To" required>
            <button type="button" class="btn btn-small btn-danger remove-time">Remove</button>
        </div>
    `;
    
    container.querySelector('.remove-time').addEventListener('click', (e) => {
        e.preventDefault();
        e.target.closest('.working-hours-item').remove();
    });
}
