$(function () {
    let bundles = [];
    let id = 0;

    function init() {

    }

    function initEvents() {
        $('#btn-add').click(() => onClickAdd());
        $('#div-bundles').on('click', '.btn-add-child', onClickAddChild);
        $('#div-bundles').on('click', '.btn-delete', onClickDelete);
    }

    function getBundleHtml(bundle) {
        // Get html of a single bundle
        let inputHtml = '';
        let styleHtml = '';

        if (bundle.level > 0) {
            styleHtml = 'margin-left:' + bundle.level * 15 + 'px;';
            inputHtml = '<input type="number" class="form-control form-control-sm input-bundle-amount" placeholder="Amount"/>';
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
                <div class="ms-auto" style="max-width: 150px;">
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

        console.log(parent);

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
        let roots = bundles.filter(bundle => bundle.level == 0);
        let html = roots.map(bundle => `<option value="${bundle.id}"> ${bundle.name} </option>`);
        $('#select-bundle').html(html);
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

    init();
    initEvents();
});