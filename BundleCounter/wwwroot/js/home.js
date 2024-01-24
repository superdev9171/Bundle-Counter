$(function () {
    let bundles = [];
    let id = 0;

    function init() {

    }

    function initEvents() {
        $('#btn-add').click(() => onClickAdd());
        $('#btn-max-count').click(onClickMaxCount);
        $('#btn-save').click(onClickSave);
        $('#btn-load').click(onClickLoad);
        $('#div-bundles').on('click', '.btn-add-child', onClickAddChild);
        $('#div-bundles').on('click', '.btn-delete', onClickDelete);
    }

    function getBundleHtml(bundle) {
        // Get html of a single bundle
        let inputHtml = '';
        let styleHtml = '';

        if (bundle.level > 0) {
            styleHtml = 'margin-left:' + bundle.level * 25 + 'px;';
            inputHtml = '<input type="number" class="form-control form-control-sm input-need-count" placeholder="Need"/>';
            inputHtml += '<input type="number" class="form-control form-control-sm input-bundle-amount ms-2 border border-success" placeholder="Amount"/>';
        }

        let bundleHtml = `
            <div class="d-flex align-items-center row-bundle bg-light p-2 border" style="${styleHtml}"  data-id="${bundle.id}">
                <div class="fw-bold">
                    ${bundle.name}
                </div>
                <div class="ms-3">
                    <a href="#" class="btn-add-child">Add</a>
                    <a href="#" class="btn-delete">Delete</a>
                </div>
                <div class="ms-auto d-flex align-items-center" style="max-width: 300px;">
                    ${inputHtml}
                </div>
            </div>
        `;
        return bundleHtml;
    }

    /*
     * Events
     */
    function onClickAdd(parent) {
        let name = prompt("Please enter name of the bundle:");

        if (!name) return;

        addBundle(name, parent);
    }

    function onClickAddChild() {
        let parentId = $(this).closest('.row-bundle').data('id');
        let parent = bundles.find(item => item.id == parentId);

        onClickAdd(parent);
    }

    function onClickDelete() {
        let id = $(this).closest('.row-bundle').data('id');

        let newBundles = [];
        bundles.forEach(bundle => {
            if (bundle.id == id)
                return;
            if (bundle.parentId >= 0) {
                if (newBundles.every(bd => bd.id != bundle.parentId))
                    return;
            }
            newBundles.push(bundle);
        });

        bundles.forEach(bundle => {
            if (newBundles.indexOf(bundle) >= 0)
                return;
            $(`.row-bundle[data-id="${bundle.id}"]`).remove();
        })

        bundles = newBundles;
        onUpdateBundle();
    }

    function onUpdateBundle() {
        // Update bundle select
        let roots = bundles.filter(bundle => bundle.level == 0);
        let html = roots.map(bundle => `<option value="${bundle.id}"> ${bundle.name} </option>`);

        $('#select-bundle').html(html);

        // Update bundle html
        bundles.forEach(bundle => {
            let hasChild = bundles.some(bd => bd.parentId == bundle.id);

            $(`.row-bundle[data-id="${bundle.id}"] .input-bundle-amount`).toggleClass('d-none', hasChild);
        });
    }

    function onClickMaxCount() {
        if (!checkAndUpdateBundleData())
            return;

        $.post('/bundle/count', {
            bundleId: $('#select-bundle').val(),
            bundles: bundles
        }).then(res => {
            if (res.success) {
                if (res.data < 0)
                    $('#div-bundle-count').text('Count: No Limit');
                else
                    $('#div-bundle-count').text(`Count: ${res.data}`);
            } else {
                alert(res['errorMessage'] || 'Operation failed');
            }
        });
    }

    function onClickSave() {
        if (!checkAndUpdateBundleData())
            return;

        $.post('/bundle', {
            bundles: bundles
        }).then(res => {
            if (res.success) {
                alert('Bundles are saved');
            } else {
                alert(res['errorMessage'] || 'Operation failed');
            }
        });
    }

    function setLevelAndLoadHtml(parent) {
        var html = getBundleHtml(parent);
        bundles.filter(bd => bd.parentId == parent.id).forEach(bd => {
            bd.level = parent.level + 1;
            html += setLevelAndLoadHtml(bd);
        });
        return html;
    }

    function onClickLoad() {

        $.get('/bundle').then(res => {
            if (res.success) {
                bundles = res.data;

                html = '';
                bundles.forEach(bundle => {
                    if (bundle.parentId < 0) {
                        bundle.level = 0;
                        html += setLevelAndLoadHtml(bundle);
                    }

                    id = Math.max(bundle.id, id) + 1;
                });

                onUpdateBundle();
                $('#div-bundles').html(html);
            } else {
                alert(res['errorMessage'] || 'Operation failed');
            }
        });
    }

    /* 
     * Operations
     */
    function addBundle(name, parent) {
        // Add a new bundle in the list

        let bundle = {
            id: id ++,
            level: parent ? parent.level + 1 : 0,
            name: name,
            amount: 0,
            parentId: parent?.id ?? -1
        };

        bundles.push(bundle);

        let html = getBundleHtml(bundle);

        if (parent)
            $(html).insertAfter(`.row-bundle[data-id="${parent.id}"]`);
        else
            $('#div-bundles').append(html);

        onUpdateBundle();
        return bundle;
    }

    function checkAndUpdateBundleData() {
        // Update bundle data

        let isValid = true;

        bundles.forEach(bundle => {
            bundle.amount = $(`.row-bundle[data-id="${bundle.id}"] .input-bundle-amount`).val() ?? 0;
            bundle.need = $(`.row-bundle[data-id="${bundle.id}"] .input-need-count`).val() ?? 0;

            if (bundle.level > 0 && bundle.need == 0)
                isValid = false;
        });

        if (!isValid) {
            alert('Please input the values.');
            return false;
        }

        return isValid;
    }

    init();
    initEvents();
});