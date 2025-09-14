// Project Estimator JavaScript utilities

// Initialize Mermaid with proper configuration
document.addEventListener('DOMContentLoaded', function() {
    if (typeof mermaid !== 'undefined') {
        mermaid.initialize({
            startOnLoad: true,
            theme: 'default',
            themeVariables: {
                primaryColor: '#0d6efd',
                primaryTextColor: '#ffffff',
                primaryBorderColor: '#0a58ca',
                lineColor: '#6c757d',
                sectionBkgColor: '#f8f9fa',
                altSectionBkgColor: '#e9ecef',
                gridColor: '#dee2e6',
                textColor: '#212529',
                taskBkgColor: '#ffffff',
                taskTextColor: '#212529',
                taskTextLightColor: '#6c757d',
                taskTextOutsideColor: '#212529',
                taskTextClickableColor: '#0d6efd',
                activeTaskBkgColor: '#cff4fc',
                activeTaskBorderColor: '#0dcaf0',
                gridGray: '#adb5bd',
                ganttNumberColor: '#495057'
            },
            gantt: {
                leftPadding: 100,
                gridLineStartPadding: 350,
                fontSize: 11,
                fontFamily: '"Segoe UI", Tahoma, Geneva, Verdana, sans-serif',
                numberSectionStyles: 4
            },
            flowchart: {
                useMaxWidth: true,
                htmlLabels: true
            }
        });
        
        // Re-render mermaid diagrams after initialization
        setTimeout(() => {
            mermaid.contentLoaded();
        }, 100);
    }
});

// Copy Mermaid code to clipboard
function copyMermaidCode(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        const textToCopy = element.textContent || element.innerText;
        
        navigator.clipboard.writeText(textToCopy).then(function() {
            showCopySuccess();
        }).catch(function(err) {
            console.error('Could not copy text: ', err);
            // Fallback for older browsers
            fallbackCopyTextToClipboard(textToCopy);
        });
    }
}

// Fallback copy method for older browsers
function fallbackCopyTextToClipboard(text) {
    const textArea = document.createElement("textarea");
    textArea.value = text;
    textArea.style.top = "0";
    textArea.style.left = "0";
    textArea.style.position = "fixed";
    
    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();
    
    try {
        const successful = document.execCommand('copy');
        if (successful) {
            showCopySuccess();
        }
    } catch (err) {
        console.error('Fallback: Oops, unable to copy', err);
    }
    
    document.body.removeChild(textArea);
}

// Show copy success feedback
function showCopySuccess() {
    const button = event.target.closest('button');
    if (button) {
        const originalText = button.innerHTML;
        button.innerHTML = '<i class="fas fa-check"></i> Copied!';
        button.classList.add('btn-success');
        button.classList.remove('btn-outline-secondary');
        
        setTimeout(() => {
            button.innerHTML = originalText;
            button.classList.remove('btn-success');
            button.classList.add('btn-outline-secondary');
        }, 2000);
    }
}

// PERT calculation for forms
function calculatePERT(optimistic, mostLikely, pessimistic) {
    const expected = (optimistic + 4 * mostLikely + pessimistic) / 6;
    const standardDeviation = Math.abs(pessimistic - optimistic) / 6;
    
    return {
        expected: expected,
        standardDeviation: standardDeviation
    };
}

// Form validation helpers
function validateThreePointEstimates(optimistic, mostLikely, pessimistic) {
    if (optimistic > pessimistic) {
        return "Optimistic hours cannot be greater than pessimistic hours.";
    }
    
    if (mostLikely < optimistic || mostLikely > pessimistic) {
        return "Most likely hours should be between optimistic and pessimistic hours.";
    }
    
    return null;
}
